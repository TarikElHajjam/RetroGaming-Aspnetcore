using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress,ErrorMessage ="Invalid Email")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password,ErrorMessage ="Invalid Password")]
        public string Password { get; set; }
        [Display(Name ="Remember me")]
        public bool RememberMe { get; set; }
        public string returnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogin { get; set; }
    }
}
