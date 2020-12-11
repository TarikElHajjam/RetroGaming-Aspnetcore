using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Retro_Gamer.Models;
using Retro_Gamer.Services;
using Retro_Gamer.ViewModels;

namespace Retro_Gamer.Controllers
{
    /// <summary>
    ///  This controller manages everything about loging in/out, signing up and sending emails to the users to
    ///  verify their emails
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly AppDbContext db;
        private readonly IEmailSender emailSender;
        private readonly IGameRepository gameRepository;
        private readonly IMemorieRepository memorieRepository;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            AppDbContext db, IEmailSender emailSender, IGameRepository gameRepository, IMemorieRepository memorieRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.db = db;
            this.emailSender = emailSender;
            this.gameRepository = gameRepository;
            this.memorieRepository = memorieRepository;
        }
        //Get AccountController/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if(signInManager.IsSignedIn(User))
            {
                return View("NotFound");
            }
            return View();
        }
        //Post AccountController/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.UserName, Email = model.Email, FullName = model.FullName };
                var Result = await userManager.CreateAsync(user, model.Password);
                if (Result.Succeeded)
                {
                    //Sends a confirmation email to the newly registered user
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
                    var message = new Message(new string[] { user.Email }, "Confirmation email link", confirmationLink);
                    await emailSender.SendEmailAsync(message);
                   
                    return RedirectToAction(nameof(SuccessRegistration));
                }
                foreach (var error in Result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        //Checks whether the given username is in use or not
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsUserNameInUse(string userName)
        {
            var InUse = await userManager.FindByNameAsync(userName);
            if (InUse == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Username {userName} is already in use");
            }
        }
        //Checks whether the given email is in use or not
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var InUse = await userManager.FindByEmailAsync(email);

            if (InUse == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }

        //Post action to log out
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //Get AcountController/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var model = new LoginViewModel()
            {
                returnUrl = returnUrl,
                ExternalLogin = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        //Post AcountController/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Using Email to login in 
                    var user = db.Users.Where(u => u.Email.Equals(model.Email)).Single(); 
                    var checkPassword = await userManager.CheckPasswordAsync(user, model.Password); 
                    if (!checkPassword)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                        return View();
                    }
                    var result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }
                catch (InvalidOperationException)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
            }
            return View(model);
        }
        //Post action to manage signing in/up with external services (i.e: Google, facebook, twitter etc...)
        [HttpPost]
        [AllowAnonymous]
         public IActionResult ExternalLogin(string provider, string returnUrl)
        {       
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        //Manages signing in/up with external services (i.e: Google, facebook, twitter etc...)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel
            {
                returnUrl = returnUrl,
                ExternalLogin = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }
            // Gets the login information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", loginViewModel);
            }

            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            // If there is no record in AspNetUserLogins table, the user may not have a local account
            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new AppUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),                     
                            NormalizedEmail = info.Principal.FindFirstValue(ClaimTypes.Email),
                            NormalizedUserName = info.Principal.FindFirstValue(ClaimTypes.Name),
                            EmailConfirmed = true,
                            PhoneNumberConfirmed = false,
                            TwoFactorEnabled = false,
                            LockoutEnabled = true,
                            AccessFailedCount = 0,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            ConcurrencyStamp = Guid.NewGuid().ToString()

                        };

                       var resultCreate = await userManager.CreateAsync(user);
                        if(resultCreate.Succeeded)
                        {    
                            // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                            var resultAddlogin = await userManager.AddLoginAsync(user, info);
                            if (resultAddlogin.Succeeded)
                            {
                                //Sends an email for the email confirmation and adds a default role as in the Register action
                                await userManager.AddToRoleAsync(user, "User");
                                await signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);
                            }
                        }                  
                    }           
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                return View("Error");
            }
        }
        //Get action to manage Email confirmation  
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return View("Error");
            }             
            var result = await userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }
        // Upon successful registration, you get a view notifiying the user of the confirmation email 
        // that has been sent to them
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SuccessRegistration()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This E-mail adresse doesn't exist");
                    return View();
                }
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var callback = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email }, "Reset password token", callback);
                await emailSender.SendEmailAsync(message);
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    RedirectToAction(nameof(NotFound));
                }
                var resetPassResult = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (!resetPassResult.Succeeded)
                {
                    foreach (var error in resetPassResult.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return View();
                }
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        public async Task <IActionResult> UserProfile()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userMemories = memorieRepository.UserSharedMemories(user.Id);         
            if (user==null)
            {
                return View("NotFound");
            }         
     
            var model = new UserProfileViewModel()
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,             
                MemoriesList = userMemories
            };
            return View(model);
        }
        [HttpPost]
        public async Task <IActionResult> UserProfile(UserProfileViewModel model)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userMemories = memorieRepository.UserSharedMemories(model.Id);
            if (user == null && user.Id != model.Id)
            {
                View("NotFound");
            }
            user.FullName = model.FullName;           
            user.UserName = model.UserName;
            model.MemoriesList = userMemories;
            var Result = await userManager.UpdateAsync(user);
            if (Result.Succeeded)
            {
                TempData["Updated"] = "Updated successfully!";
                return RedirectToAction("UserProfile");
            }
            foreach (var error in Result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }       
            return View(model);
        }
        [HttpGet]
        public IActionResult ChangeEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return View("NotFound");
                }
                var checkEmail = await userManager.FindByEmailAsync(model.NewEmail);
                if(checkEmail != null)
                {
                    TempData["AlreadyInUseEmail"] = "This email already exists";
                    ModelState.AddModelError(string.Empty, "This email exists");
                    return View();
                }
        
                var token = await userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                var callback = Url.Action(nameof(EmailChangedConfirmation), "Account", new { token, email = model.NewEmail }, Request.Scheme);
                var message = new Message(new string[] { user.Email }, "Change Email confirmation", callback);
                await emailSender.SendEmailAsync(message);
                TempData["ChangeEmail"] = "An email confirmation link has been sent to email successfully!";
                return RedirectToAction(nameof(UserProfile));
            }
            return View(model);
        }
        [HttpGet]
        public  async Task<IActionResult>  EmailChangedConfirmation(string token, string email)
        {
            var model = new ChangeEmailViewModel { Token = token, NewEmail = email };
            var user = await userManager.GetUserAsync(User);
            if (user == null && signInManager.IsSignedIn(User))
            {
                return View("NotFound");
            }
            var changeEmail = await userManager.ChangeEmailAsync(user, model.NewEmail, model.Token);
            if (!changeEmail.Succeeded)
            {
                foreach (var error in changeEmail.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }
            TempData["EmailHasChanged"] = "Your email has been updated successfully!";
            return RedirectToAction(nameof(UserProfile));
        }
     
        [HttpGet]
        public IActionResult ChangePassword()
        {  
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return View("NotFound");
                }
                var checkPassowrd = await userManager.CheckPasswordAsync(user, model.Passowrd);
                if (!checkPassowrd)
                {
                    ModelState.AddModelError(string.Empty, "Current Passowrd is incorrect");
                    return View();
                }
                var Result = await userManager.ChangePasswordAsync(user, model.Passowrd, model.NewPassowrd);
                if (!Result.Succeeded)
                {
                    foreach (var error in Result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }
                await signInManager.RefreshSignInAsync(user);
                TempData["Passowrd Changed"] = "Your password has been changed successfully";
                return RedirectToAction("UserProfile", "Account");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult>  AddPasswordExternalLogin()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            if(await userManager.HasPasswordAsync(user))
            {
                return View("NotFound");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddPasswordExternalLogin(AddPasswordExternalLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                var result = await userManager.AddPasswordAsync(user, model.ConfirmPassword);

                if(!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }
                TempData["Password had been added"] = "Your password has been added successfully!";
                return RedirectToAction("UserProfile", "Account");
            }
            return View(model);
        }
    }
}
