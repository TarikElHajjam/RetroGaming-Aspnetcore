using Microsoft.AspNetCore.Identity;
using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }
        public string Id { get; set; }
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Full name")]
        public string Fullname { get; set; }
        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }

    }
}
