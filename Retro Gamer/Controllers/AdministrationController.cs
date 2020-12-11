using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Retro_Gamer.Models;
using Retro_Gamer.Services;
using Retro_Gamer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Retro_Gamer.Controllers
{

    [Authorize(Policy = "SuperAdminPolicy")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<AdministrationController> logger;
        private readonly UserManager<AppUser> userManager;
        private readonly IPendingGameRepository pendingGameRepository;
        private readonly IGameRepository gameRepository;
        private readonly IEmailSender emailSender;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IMemorieRepository memorieRepository;

        public AdministrationController(RoleManager<IdentityRole> roleManager, ILogger<AdministrationController> logger, UserManager<AppUser> userManager,
            IPendingGameRepository pendingGameRepository, IGameRepository gameRepository, IEmailSender emailSender, SignInManager<AppUser> signInManager,
            IMemorieRepository memorieRepository)
        {
            this.roleManager = roleManager;
            this.logger = logger;
            this.userManager = userManager;
            this.pendingGameRepository = pendingGameRepository;
            this.gameRepository = gameRepository;
            this.emailSender = emailSender;
            this.signInManager = signInManager;
            this.memorieRepository = memorieRepository;
        }
        [HttpGet]
        public IActionResult CreateRoles()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoles(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole
                {
                    Name = model.Name
                };

                IdentityResult result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        //Checks wethere the given name of the newly created role is in use or not
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsRoleInUse(string name)
        {
            var InUse = await roleManager.FindByNameAsync(name.ToUpper());
            if (InUse == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Username {name} is already use");
            }
        }
        //Shows the list of all the roles
        [HttpGet]
        public IActionResult ListRoles()
        {
            var List = roleManager.Roles;
            if (List == null)
            {
                return View("NotFound");
            }
            return View(List);
        }

      
        [HttpGet]
        public async Task<ViewResult> EditRoles(string Id)
        {

            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach (var user in userManager.Users.ToList())
            {

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }
        //Edits the role name
        [HttpPost]

        public async Task<ActionResult> EditRoles(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                return View("NotFound");
            }
            else
            {
                role.Id = model.Id;
                role.Name = model.RoleName;
                var Result = await roleManager.UpdateAsync(role);
                if (Result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach (var error in Result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
        [HttpGet]
        public async Task<ViewResult> DeleteRoles(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var roledelete = new DeleteRolesViewModel
                {
                    Id = role.Id,
                    Name = role.Name
                };
                return View(roledelete);
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRoles(IdentityRole user)
        {
            var role = await roleManager.FindByIdAsync(user.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {user.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError($"Exception Occured : {ex}");
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role." +
                        $" If you want to delete it, please remove the users from the role and then try to delete it again";
                    return View("Error");
                }
            }
        }
        //Shows the users assigned to the role 
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("Error");
            }
            var model = new List<UsersRolesViewModel>();
            foreach (var user in await userManager.Users.ToListAsync())
            {
                var userRoleViewModel = new UsersRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }
            return View(model);
        }
        //Edits the users assigned to the role 
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UsersRolesViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;
                //checks if the user has been selected, if it is true then it adds the selected user to the role 
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRoles", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRoles", new { Id = roleId });
        }
        [HttpGet]
        public IActionResult ListUsers()
        {
            var user = userManager.Users;
            var role = roleManager.Roles;
            var getClaims = new ClaimsIdentity();
            var claim = getClaims.Claims;
            var model = new ListUsersViewModel()
            {
                User = user,
                Role = role,
                Claim = claim
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("NotFound");
            }
            var userClaims = await userManager.GetClaimsAsync(user);

            var userRoles = await userManager.GetRolesAsync(user);
            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                Fullname = user.FullName,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles

            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return RedirectToAction("NotFound");
            }
            else
            {
                user.Id = model.Id;
                user.Email = model.Email;
                user.UserName = model.Username;
                user.FullName = model.Fullname;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                return RedirectToAction("ListUsers", "Administration");
            }
        }
        [HttpGet]

        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("AccessDenied");
            }
            var model = new EditUserViewModel()
            {
                Email = user.Email,
                Fullname = user.FullName,
                Username = user.UserName
            };
            return View(model);
        }
        ///Deletes everything about the user
        [HttpPost]
        public async Task<IActionResult> DeleteUser(EditUserViewModel userId)
        {
            var user = await userManager.FindByIdAsync(userId.Id);

            if (user == null)
            {
                return View("AccessDenied");
            }
            //deletes user's shared memories
            var allMemories = memorieRepository.UserSharedMemories(user.Id);
            if (allMemories != null)
            {
                foreach (var m in allMemories.ToList())
                {
                    await memorieRepository.DeleteMemorie(m);
                }
            }
            //deletes user's login (Google)
            var userLoginInfo = await userManager.GetLoginsAsync(user);

            if (userLoginInfo != null)
            {
                foreach (var info in userLoginInfo)
                {
                    var userByInfo = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                    if (userByInfo.Id == user.Id)
                    {
                        await userManager.RemoveLoginAsync(userByInfo, info.LoginProvider, info.ProviderKey);
                    }
                }
            }
            //Remove user's claims
            var userClaims = await userManager.GetClaimsAsync(user);          
            if (userClaims != null)
            {
                await userManager.RemoveClaimsAsync(user, userClaims);
            }
            //Remove user from roles
            await RemoveUserFromRoles(user);        
            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("ListUsers");
        }
        [NonAction]
        private async Task<IdentityResult> RemoveUserFromRoles(AppUser user)
        {
            var userInRoles = await userManager.GetRolesAsync(user);
            if (userInRoles == null)
            {
                return null;
            }
            return await userManager.RemoveFromRolesAsync(user, userInRoles); ;       
        }

        [HttpGet]
       
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("NotFound");
            }
            var model = new List<UsersRolesViewModel>();
            foreach (var role in roleManager.Roles.ToList())
            {
                var userRoles = new UsersRolesViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoles.IsSelected = true;
                }
                else
                {
                    userRoles.IsSelected = false;
                }
                model.Add(userRoles);
            }
            return View(model);
        }
        [HttpPost]

        public async Task<IActionResult> ManageUserRoles(List<UsersRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
           
            return RedirectToAction("EditUser", new { userId = userId });
        }
        [HttpGet]

        public async Task<IActionResult> ManageUserSClaims(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return View("NotFound");
            }
            var existingUserClaims = await userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel()
            {
                UserId = user.Id
            };
            foreach (Claim claim in ClaimsStore.AllClaims.ToList())
            {
                var userClaims = new UserClaim()
                {
                    ClaimType = claim.Type,

                };
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaims.IsSelected = true;
                }
                model.Claims.Add(userClaims);
            }
            return View(model);
        }
        [HttpPost]

        public async Task<IActionResult> ManageUsersClaims(UserClaimsViewModel model, string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return View("NotFound");
            }
            var getClaims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, getClaims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }
            result = await userManager.AddClaimsAsync(user, model.Claims.Select
                (c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }
            return RedirectToAction("EditUser", new { userId = id });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ManagePendingGames()
        {
            var allPendingGames = pendingGameRepository.GetAllGames();

            var model = new List<ManagePendingGamesViewModel>();

            foreach (var pg in allPendingGames.ToList())
            {
                var pendingGame = new ManagePendingGamesViewModel()
                {

                    GameId = pg.Id,
                    Description = pg.Description,
                    UserId = pg.UserId,
                    Name = pg.Name,
                    Genres = pg.Genres,
                    Rating = pg.Rating,
                    RelaseDate = pg.RelaseDate,
                    PhotoUrl = pg.PhotoUrl
                };
                model.Add(pendingGame);
            }
            return View(model.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> ManagePendingGames(int[] gameId)
        {
            if (ModelState.IsValid)
            {
                foreach (var pg in gameId.ToList())
                {
                    var ApprovedGame = pendingGameRepository.GetById(pg);
                    if (ApprovedGame == null)
                    {
                        ModelState.AddModelError(string.Empty, "The game Id is not valid");
                        return View();
                    }
                    {
                        var game = new Game()
                        {
                            Description = ApprovedGame.Description,
                            Name = ApprovedGame.Name,
                            Genres = ApprovedGame.Genres,
                            Rating = ApprovedGame.Rating,
                            RelaseDate = ApprovedGame.RelaseDate,
                            PhotoUrl = ApprovedGame.PhotoUrl
                        };

                        gameRepository.AddNewGame(game);
                        //Sends a notification via email to notify the user that his/her game has been approved
                        var user = await userManager.FindByIdAsync(ApprovedGame.UserId);
                        var callback = Url.Action("details", "home", new { id = game.Id }, Request.Scheme);
                        var message = new Message(new string[] { user.Email }, "Your game has been approved, check the attached link:", callback);
                        await emailSender.SendEmailAsync(message);
                        //Deletes the approved game 
                        pendingGameRepository.Delete(ApprovedGame.Id);
                        TempData["Approved"] = "The selected games have been added to the Game List";
                    }

                }
                return RedirectToAction("ManagePendingGames", "Administration");
            }
            return RedirectToAction("Games", "home");
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var user = await userManager.GetUserAsync(User);
      
            if (user == null)
            {
                return View("NotFound");
            }
            var rolesList = roleManager.Roles.ToList();
            var model = new CreateUserViewModel()
            {
                AllRoles = rolesList
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model, string [] roleId)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true,
                    FullName = model.FullName
                };
                var Result = await userManager.CreateAsync(user, model.Password);
                if (!Result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create a new user");
                    return View();
                }
                foreach (var r in roleId)
                {
                    await userManager.AddToRoleAsync(user, r);
                }
                return RedirectToAction("EditUser", new { id = user.Id });
            }
            return View(model);
        }

    }
}