using Microsoft.AspNetCore.Identity;
using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class ListUsersViewModel
    {
        public IEnumerable<Claim> Claim { get; set; }
        public IEnumerable<IdentityRole> Role { get; set; }
        public IEnumerable<AppUser> User { get; set; }
    }
}
