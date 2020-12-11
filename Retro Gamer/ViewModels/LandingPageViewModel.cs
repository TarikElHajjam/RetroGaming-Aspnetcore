using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class LandingPageViewModel
    {
        public IEnumerable<Memories> GetAllMemories { get; set; }
        public IEnumerable<Game> GameList { get; set; }
        public PaginatedList<Game> PaginatedList { get; set; }

    }
}
