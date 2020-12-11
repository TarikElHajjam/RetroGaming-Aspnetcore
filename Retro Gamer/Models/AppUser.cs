using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
                
        }
        public string FullName { get; set; }
    }
}
