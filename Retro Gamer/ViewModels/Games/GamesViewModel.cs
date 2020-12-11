using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class GamesViewModel
    {
        public IEnumerable<Game> GameList { get; set; }
        public PaginatedList<Game> PaginatedList { get; set; }
    }
}
