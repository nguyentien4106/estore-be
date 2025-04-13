using BuildingBlocks.Auth.Constants;
using BuildingBlocks.Auth.Models;
using BuildingBlocks.Models;
using Microsoft.AspNetCore.Authorization;

namespace EStore.Api.Middlewares.Auth;

public class AccountAuthorizationHandler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<AccountRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountRequirement requirement)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if(httpContext == null)
        {
            context.Fail(new AuthorizationFailureReason(this, "Unable to access HTTP context."));
            return Task.CompletedTask;
        }

        if(requirement.AccountType == AccountType.Pro.ToString())
        {
            if (!context.User.HasClaim(ClaimNames.AccountType, AccountType.Pro.ToString()))
            {
                context.Fail(new AuthorizationFailureReason(this, "You do not have the required permissions. Upgrade to the Pro tier to access this feature."));
                httpContext.Items["AuthorizationFailure"] = new AppResponse<string>(){
                    Succeed = false,
                    Data = "ACCOUNT_TYPE_NOT_ALLOWED",
                    Message = "You do not have the required permissions. Upgrade to the Pro tier to access this feature."
                };

                return Task.CompletedTask;
            }
        }

        if(requirement.AccountType == AccountType.Plus.ToString())
        {
            if (!context.User.HasClaim(ClaimNames.AccountType, AccountType.Plus.ToString()))
            {
                context.Fail(new AuthorizationFailureReason(this, "You do not have the required permissions. Upgrade to the Plus tier to access this feature."));
                httpContext.Items["AuthorizationFailure"] = new AppResponse<string>(){
                    Succeed = false,
                    Data = "ACCOUNT_TYPE_NOT_ALLOWED",
                    Message = "You do not have the required permissions. Upgrade to the Plus tier to access this feature."
                };

                return Task.CompletedTask;
            }
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}