using EStore.Application.Helpers;
using EStore.Domain.Models;
using BuildingBlocks.Auth.Models;
using EStore.Application.Data;

namespace EStore.Application.Auth.Commands.Auth.RefreshToken;

public class RefreshTokenHandler(
    JwtSettings jwtSettings, 
    UserManager<User> userManager, 
    IEStoreDbContext context
) : ICommandHandler<RefreshTokenCommand, AppResponse<AuthToken>>
{
    public async Task<AppResponse<AuthToken>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var user = await TokenUtils.GetUserFromRefreshToken(context, command.RefreshToken);
        if(user is null)
        {
            return AppResponse<AuthToken>.Error("User Not Found.");
        }

        if (!TokenUtils.IsRefreshTokenValid(user.RefreshTokenExpiry))
        {
            return AppResponse<AuthToken>.Error("Your session has expiry");
        }

        // Generate new tokens
        return AppResponse<AuthToken>.Success(new()
        {
            RefreshToken = await TokenUtils.GenerateRefreshToken(jwtSettings, user, context),
            AccessToken = await TokenUtils.GenerateAccessToken(userManager, jwtSettings, user)
        });
    }
}