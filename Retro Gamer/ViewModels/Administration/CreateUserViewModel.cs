using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class CreateUserViewModel : RegisterViewModel
    {
        public CreateUserViewModel()
        {
            this.AllRoles = new List<IdentityRole>();
        }
        public List<IdentityRole> AllRoles { get; set; }
        public bool IsSelected { get; set; }
    }
}
