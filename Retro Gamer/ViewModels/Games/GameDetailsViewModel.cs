using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class GameDetailsViewModel
    {
        public Memories Memories { get; set; }
        public IEnumerable<Memories> AllMemories { get; set; }
        public IEnumerable<Game> AllGames { get; set; }
        public Game Game { get; set; }
        public string PageTitle { get; set; }


    }
}
