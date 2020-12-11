using Microsoft.AspNetCore.Http;
using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class GameCreateViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime RelaseDate { get; set; }
        [Required(ErrorMessage = "Genres field is required")]
        public Genre Genres { get; set; }
        [Required, Range(1, 5)]
        public int Rating { get; set; }
        [Required, MaxLength(220), MinLength(50)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please upload a photo")]
        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(extensions: new[] { ".jpg", ".png" })]
        public IFormFile Photo { get; set; }
        public bool IsApproved { get; set; }

    }
}
