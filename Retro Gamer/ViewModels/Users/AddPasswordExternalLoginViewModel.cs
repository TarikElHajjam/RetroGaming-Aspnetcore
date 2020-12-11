using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class AddPasswordExternalLoginViewModel
    {
        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password" )]
        [Compare("NewPassword", ErrorMessage = "The passwords do not match!")]
        public string ConfirmPassword { get; set; }
        public AppUser User { get; set; }
    }
}
