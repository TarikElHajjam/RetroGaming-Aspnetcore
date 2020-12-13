using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Retro_Gamer.Models;
using Retro_Gamer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Retro_Gamer.Services;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;
using Amazon.Runtime;
using Microsoft.AspNetCore.Http;

namespace Retro_Gamer.Controllers
{

    public class HomeController : Controller
    {
        private readonly IGameRepository gameRepository;
        private readonly IMemorieRepository memorieRepository;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IAmazonS3Bucket uploadImage;

        public HomeController(IGameRepository gameRepository, IMemorieRepository memorieRepository, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IAmazonS3Bucket uploadImage)
        {
            this.gameRepository = gameRepository;
            this.memorieRepository = memorieRepository;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.uploadImage = uploadImage;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? pageNumber, string searchString)
        {
            var games = gameRepository.Search(searchString);
            var memories = memorieRepository.GetAllMemories();
            int pageSize = 6;
            var paginatedList = await PaginatedList<Game>.CreateAsync(games, pageNumber ?? 1, pageSize);
            var model = new LandingPageViewModel()
            {
                GameList = games,
                GetAllMemories = memories,
                PaginatedList = paginatedList
            };
            return View(model);
        }
        //Details view
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Details(int id)
        {
            var allMemories = memorieRepository.GetAllMemories();
            var game = gameRepository.GetById(id);
            var allGames = gameRepository.GetAllGames();

            if (game == null)
            {
                return View("NotFound");
            }
            HomeDetailsViewModel model = new HomeDetailsViewModel()
            {
                Game = game,
                AllMemories = allMemories,
                AllGames = allGames,
                PageTitle = "Game's Details"
            };
            if (allMemories != null)
            {
                foreach (var m in allMemories.ToList())
                {
                    model.ListMemories.Add(m);
                }
                return View(model);
            }
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id, HomeDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var game = gameRepository.GetById(id);
                var user = await userManager.GetUserAsync(HttpContext.User);
                if (game == null)
                {
                    Response.StatusCode = 404;
                    return View("NotFound");
                }
                if (user == null)
                {
                    return View("NotFound");
                }
                Memories memories = new Memories()
                {
                    gameId = game.Id,
                    Memory = model.Memories.Memory,
                    UserId = user.Id,
                    UserName = user.UserName,
                    DatePosted = DateTime.Now
                };
                memorieRepository.AddMemories(memories);
                return RedirectToAction("details", new { id = game.Id });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (user != null && signInManager.IsSignedIn(User))
            {
                if (!(await userManager.IsInRoleAsync(user, "Admin")))
                {
                    return RedirectToAction("Create", "PendingGame");
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(GameCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var photoUrl = await uploadImage.UploadImage(model.Photo);
                var game = new Game
                {
                    Name = model.Name,
                    Description = model.Description,
                    Genres = model.Genres,
                    PhotoUrl = photoUrl,
                    Rating = model.Rating,
                    RelaseDate = model.RelaseDate
                };
                gameRepository.AddNewGame(game);
                return RedirectToAction("details", new { id = game.Id });
            }
            return View();
        }
        [HttpGet]
        [Authorize(Policy = "GameEditorPolicy")]
        public ViewResult Edit(int id)
        {
            var game = gameRepository.GetById(id);
            if (game == null)
            {
                return View("NotFound");
            }
            GameEditViewModel model = new GameEditViewModel
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description,
                Genres = game.Genres,
                RelaseDate = game.RelaseDate,
                ExistingPhoto = game.PhotoUrl,
                Rating = game.Rating
            };
            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "GameEditorPolicy")]
        public async Task<IActionResult> Edit(GameEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Game game = gameRepository.GetById(model.Id);
                game.Name = model.Name;
                game.Description = model.Description;
                game.Genres = model.Genres;
                game.Rating = model.Rating;
                game.RelaseDate = model.RelaseDate;
                if (model.Photo != null)
                {
                    if (model.ExistingPhoto != null)
                    {
                        await uploadImage.Delete(model.ExistingPhoto);
                    }
                    game.PhotoUrl = await uploadImage.UploadImage(model.Photo);
                }
                gameRepository.Update(game);
                return RedirectToAction("details", new { id = game.Id });
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "GameEditorPolicy")]
        public ViewResult Delete(int id)
        {
            var game = gameRepository.GetById(id);
            if (game == null)
            {
                return View("NotFound");
            }

            GameDeleteViewModel model = new GameDeleteViewModel
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description,
                Genres = game.Genres,
                RelaseDate = game.RelaseDate,
                ExistingPhoto = game.PhotoUrl,
                Rating = game.Rating
            };
            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "GameEditorPolicy")]
        public async Task<IActionResult> Delete(int? id)
        {
            Game game = gameRepository.GetById(id.Value);
            var memories = memorieRepository.GetAllMemories();
            foreach (var m in memories)
            {
                if (m.gameId == game.Id)
                {
                    await memorieRepository.DeleteMemorie(m);
                }
            }
            await uploadImage.Delete(game.PhotoUrl);
            gameRepository.Delete(game.Id);
            return RedirectToAction("Games", "home");
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Games(string searchString, int? pageNumber)
        {
            var games = gameRepository.Search(searchString);
            int pageSize = 6;
            var paginatedList = await PaginatedList<Game>.CreateAsync(games, pageNumber ?? 1, pageSize);
            var model = new GamesViewModel()
            {
                GameList = games,
                PaginatedList = paginatedList
            };
            return View(model);
        }
        //search game and display the games 
        [HttpGet]
        [AllowAnonymous]
        public PartialViewResult SearchGame()
        {
            return PartialView();
        }

    }
}
