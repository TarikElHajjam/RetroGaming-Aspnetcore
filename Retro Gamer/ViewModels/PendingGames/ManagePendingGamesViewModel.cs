using Microsoft.AspNetCore.Mvc;
using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class ManagePendingGamesViewModel
    {
      
        public IEnumerable <PendingGame> PendingGamesList { get; set; }
        public int GameId { get; set; }

        public string Name { get; set; }


        public DateTime RelaseDate { get; set; }

        public Genre Genres { get; set; }

        public int Rating { get; set; }

        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public string UserId { get; set; }
      [Required(ErrorMessage = "This action can not be done without you selecting an item first")]
        public bool IsSelected { get; set; }


    }
}
