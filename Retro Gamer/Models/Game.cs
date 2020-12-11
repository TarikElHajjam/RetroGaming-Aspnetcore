using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime RelaseDate { get; set; }
        [Required]
        public Genre Genres { get; set; }
        [Required, Range(1, 5)]
    
        public int Rating { get; set; }
        [Required,MaxLength(220),MinLength(50)]
        public string Description { get; set; }
        public string PhotoUrl { get; set; }

    }
}
