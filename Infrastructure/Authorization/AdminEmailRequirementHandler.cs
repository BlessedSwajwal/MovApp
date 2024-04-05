using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Infrastructure.Authorization;
public class AdminEmailRequirementHandler : AuthorizationHandler<AdminEmailRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminEmailRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == UserRoles.admin))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
