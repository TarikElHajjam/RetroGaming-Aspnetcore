using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class MemoriesCreateViewModel
    {
        [Key]
        public int Id { get; set; }
        public AppUser Users { get; set; }
        [MinLength(50), MaxLength(600), Required]
        public string Memory { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Posted at")]
        public Game Game { get; set; }
        public DateTime DatePosted { get; set; }
    }
}
