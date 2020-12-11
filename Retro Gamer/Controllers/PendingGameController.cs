using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Retro_Gamer.Models;
using Retro_Gamer.Services;
using Retro_Gamer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Controllers
{
    public class PendingGameController : Controller
    {
        private readonly IPendingGameRepository pendingGameRepository;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;


        public PendingGameController(IPendingGameRepository pendingGameRepository,
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.pendingGameRepository = pendingGameRepository;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await userManager.GetUserAsync(User);
            if(user !=null && signInManager.IsSignedIn(User) && 
                await userManager.IsInRoleAsync(user,"Super Admin"))
            {
                return RedirectToAction("Create", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> Create(PendingGameCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                if (user == null)
                {
                    return View("NotFound");
                }
                var uploadImage = new AmazonS3Bucket();
               
                string uniqueFileName = await uploadImage.UploadImage(model.Photo);
                var PendingGame = new PendingGame()
                {
                    Id = model.Id,
                    Genres = model.Genres,
                    Description = model.Description,
                    RelaseDate = model.RelaseDate,
                    Rating = model.Rating,
                    Name = model.Name,
                    UserId = user.Id,
                    PhotoUrl = uniqueFileName
                };
                TempData["Added"] = "Thank you for adding a new game! It's now awaits aprroval by our team, once approved, you will get notified by an Email!";
                pendingGameRepository.AddGame(PendingGame);
                return RedirectToAction("Games", "home");
            }         
            return View();
        }

    }
}
