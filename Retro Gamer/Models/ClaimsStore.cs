using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role","Create Role"),
            new Claim("Edit Role","Edit Role"),
            new Claim("Delete Role","Delete Role"),
            new Claim("Create Game","Create Game"),
            new Claim("Edit Game","Edit Game"),
            new Claim("Delete Game","Delete Game")
        };          
    }
}
