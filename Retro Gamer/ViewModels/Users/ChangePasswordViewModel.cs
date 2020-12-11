using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class ChangePasswordViewModel
    {   [Required]
        [DataType(DataType.Password)]
        public string Passowrd { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassowrd { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassowrd", ErrorMessage = "Password does not match")]
        public string ConfirmPassowrd { get; set; }
        public AppUser User { get; set; }
    }
}
