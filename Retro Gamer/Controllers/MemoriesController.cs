using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Retro_Gamer.Models;
using Retro_Gamer.ViewModels;

namespace Retro_Gamer.Controllers
{
    public class MemoriesController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly IGameRepository gameRepository;
        private readonly IMemorieRepository memorieRepository;

        public MemoriesController(AppDbContext _context, UserManager<AppUser> userManager, IMemorieRepository memorieRepository, IGameRepository gameRepository)
        {
            context = _context;
            this.userManager = userManager;
            this.gameRepository = gameRepository;
            this.memorieRepository = memorieRepository;
        }

        // GET: MemoriesController/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            var Memory = memorieRepository.GetMemorieById(id);
            if (Memory == null)
            {             
                return View("NotFound");
            }
            var model = new MemoriesDetailsViewModel
            {
                Memories = Memory
            };
            return View(model);
        }

        // GET: MemoriesController/Create
        [HttpGet]
        public ActionResult Create(int id)
        {
            var game = gameRepository.GetById(id);
            var model = new MemoriesCreateViewModel()
            {
                Game = game,
            };
            return View(model);
        }

        // POST: MemoriesController/Create
        [HttpPost]

        public async Task <ActionResult> Create(int id, MemoriesCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var game = gameRepository.GetById(id);
                var user = await userManager.GetUserAsync(HttpContext.User);
                if (game == null)
                {
                    return View("NotFound");
                }
                if (user == null)
                {
                    return View("NotFound");
                }
                Memories Memory = new Memories()
                {
                    gameId = game.Id,
                    Memory = model.Memory,
                    UserId = user.Id,
                    UserName = user.UserName,
                    DatePosted = DateTime.Now
                };
                memorieRepository.AddMemories(Memory);
                return RedirectToAction("Details", "Home", new { id = game.Id });
            }
            return View(model);
        }

        // GET: MemoriesController/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {     //Returns edit view
            var Memory = await context.DbMemories.FindAsync(id);
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (Memory == null || Memory.UserId != user.Id)
            {
                return View("NotFound");
            }
            var model = new MemoriesEditViewModel()
            {
                Memories = Memory
            };
            return View(model);
        }

        // POST: MemoriesController/Edit/5
        [HttpPost]

        public async Task<ActionResult> Edit(int id, MemoriesEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Gets the current user   
                var user = await userManager.GetUserAsync(HttpContext.User);       
                var Memory = memorieRepository.GetMemorieById(id);
                // Maps the model memory to Memories edit view model 
                Memory.Memory = model.Memories.Memory;
                Memory.User = user;
                Memory.UserName = user.UserName;
                await context.SaveChangesAsync();
                return RedirectToAction("details", "home", new { id = Memory.gameId });
            }
            return View(model);
        }

        // GET: MemoriesController/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var Memory = memorieRepository.GetMemorieById(id);
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (Memory == null || Memory.UserId != user.Id)
            {
                return View("NotFound");
            }
            MemoriesDeleteViewModel model = new MemoriesDeleteViewModel()
            {
                Memories = Memory
            };
            return View(model);
        }

        // POST: MemoriesController/Delete/5
        [HttpPost]

        public async Task<ActionResult> Delete(int? id)
        {
            if (ModelState.IsValid)
            {
                var Memory = memorieRepository.GetMemorieById(id.Value);
                MemoriesDeleteViewModel model = new MemoriesDeleteViewModel()
                {
                    Memories = Memory
                };

                context.DbMemories.Remove(model.Memories);
                await context.SaveChangesAsync();
                return RedirectToAction("Games", "home");
            }
            return RedirectToAction("Games", "home");
        }
    }
}
