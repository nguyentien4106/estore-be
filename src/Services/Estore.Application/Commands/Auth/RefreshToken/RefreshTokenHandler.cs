using EStore.Application.Helpers;
using BuildingBlocks.Auth.Models;
using Estore.Application.Models.Dtos;

namespace EStore.Application.Commands.Auth.RefreshToken;

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

        var accessToken = await TokenUtils.GenerateAccessToken(userManager, jwtSettings, user);
        var refreshToken = await TokenUtils.GenerateAccessToken(userManager, jwtSettings, user);
        
        // Generate new tokens
        return AppResponse<AuthToken>.Success(new(accessToken, refreshToken));
    }
}