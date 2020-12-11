using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Remote(action:"IsEmailInUse", controller:"Account")]
        [EmailAddress]
        public string Email { get; set; }
        [Required] 
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password",ErrorMessage ="Password does not match")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Remote(action:"IsUserNameInUse", controller:"Account")]
        [Display(Name ="Username")]
        [MaxLength(20, ErrorMessage = "Username lenght is over the characters limit")]
        [MinLength(4, ErrorMessage = "Username has to have at least 4 charachters")]
        public string UserName { get;  set; }
        [Required]
        [Display(Name ="Full Name")]
        [MaxLength(70, ErrorMessage = "Full Name lenght is over the characters limit")]
        [MinLength(4, ErrorMessage = "Please write a valid full name")]
        public string FullName { get;  set; }
    }
}
