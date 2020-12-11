using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class UserProfileViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "This field can not be left empty")]
        [MaxLength(70, ErrorMessage = "Full name lenght is over the characters limit which 70 characters")]
        [MinLength(4, ErrorMessage = "Full name has to have at least 4 charachters")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "This field can not be left empty")]
        [MaxLength(20, ErrorMessage = "Username lenght is over the characters limit which 20 characters")]
        [MinLength(4, ErrorMessage = "Username has to have at least 4 charachters")]
        public string UserName { get; set; }
        public string Email { get; set; } 
        public IEnumerable<Memories> MemoriesList { get; set; }

    }
}
