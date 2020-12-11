using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class HomeDetailsViewModel
    {
        public HomeDetailsViewModel()
        {
          
           
            this.ListMemories = new List<Memories>();
          
        }
     
        public Memories Memories { get; set; }
        public List<Memories> ListMemories { get; set; }
        public IEnumerable<Memories> AllMemories { get; set; }
        public IEnumerable<Game> AllGames { get; set; }
        public Game Game { get; set; }
        public string PageTitle { get; set; }


    }
}
