using EStore.Application.Helpers;
using EStore.Domain.Models;
using BuildingBlocks.Auth.Constants;
using BuildingBlocks.Auth.Models;

namespace EStore.Application.Auth.Commands.Auth.RefreshToken;

public class RefreshTokenHandler(JwtSettings jwtSettings, UserManager<User> userManager) : ICommandHandler<RefreshTokenCommand, AppResponse<EStoreToken>>
{
    public async Task<AppResponse<EStoreToken>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var principal = TokenUtils.GetPrincipalFromExpiredToken(jwtSettings, command.AccessToken);
        var userName = principal.FindFirst(ClaimNames.UserName)?.Value;
        
        if (userName is null)
        {
            return AppResponse<EStoreToken>.Error("Invalid access token");
        }

        var user = await userManager.FindByNameAsync(userName);

        if (user is null)
        {
            return AppResponse<EStoreToken>.Error("User not found!.");
        }
        
        if (!await userManager.VerifyUserTokenAsync(user, jwtSettings.RefreshTokenProvider, "RefreshToken", command.RefreshToken))
        {
            return AppResponse<EStoreToken>.Error("Refresh token expired");
        }

        return AppResponse<EStoreToken>.Success(new()
        {
            RefreshToken = await TokenUtils.GenerateRefreshToken(userManager, jwtSettings, user),
            AccessToken = await TokenUtils.GenerateAccessToken(userManager, jwtSettings, user)
        });
    }
}