using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Retro_Gamer.Security
{
    public class ManageGames : AuthorizationHandler<ManageWhoCanManageGamesRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ManageGames(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
      
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
         ManageWhoCanManageGamesRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (context.User.IsInRole("Admin") || context.User.IsInRole("Moderator"))
            {
                if (context.User.HasClaim(claim => claim.Type == "Create Game" && claim.Value == "true") &&
                    context.User.HasClaim(claim => claim.Type == "Edit Game" && claim.Value == "true") &&
                    context.User.HasClaim(claim => claim.Type == "Delete Game" && claim.Value == "true"))
                {
                    context.Succeed(requirement);
                }

            }
            return Task.CompletedTask;
        }
    }
}
