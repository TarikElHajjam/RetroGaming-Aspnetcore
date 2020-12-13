using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Retro_Gamer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Retro_Gamer.Security
{
    /// <summary>
    /// It can help to prevent an admin to edit or delete other admin or itself 
    ///    You can use it like this:  [Authorize("ForbiddenPolicy")]
    /// </summary>
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler :
         AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {  
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
         ManageAdminRolesAndClaimsRequirement requirement)
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;
            if (authFilterContext == null)
            {
                return Task.CompletedTask;
            }

            string loggedInAdminId =
                context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string user = "56f26c9b-bd7b-42d2-80f7-d29ced92ba9d";
            string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];

            if (context.User.IsInRole("Admin") &&
                context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
                adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower() && adminIdBeingEdited.ToLower() != user.ToLower())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
 
    }
}
