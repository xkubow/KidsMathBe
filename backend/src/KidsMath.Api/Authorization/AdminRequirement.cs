using Microsoft.AspNetCore.Authorization;

namespace KidsMath.Api.Authorization;

public class AdminRequirement : IAuthorizationRequirement;

public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRequirement requirement)
    {
        var tokenType = context.User.FindFirst("token_type")?.Value;
        if (tokenType is "admin" or "parent")
        {
            var isAdmin = context.User.FindFirst("is_admin")?.Value == "true";
            if (isAdmin) context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
