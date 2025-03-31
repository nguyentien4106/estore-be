using EStore.Application.Helpers;
using EStore.Domain.Models;
using BuildingBlocks.Auth.Constants;
using BuildingBlocks.Auth.Models;

namespace EStore.Application.Auth.Commands.Auth.RefreshToken;

public class RefreshTokenHandler(JwtSettings jwtSettings, UserManager<User> userManager) : ICommandHandler<RefreshTokenCommand, AppResponse<AuthToken>>
{
    public async Task<AppResponse<AuthToken>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var principal = TokenUtils.GetPrincipalFromExpiredToken(jwtSettings, command.AccessToken);
        var userName = principal.FindFirst(ClaimNames.UserName)?.Value;
        
        if (userName is null)
        {
            return AppResponse<AuthToken>.Error("Invalid access token");
        }

        var user = await userManager.FindByNameAsync(userName);

        if (user is null)
        {
            return AppResponse<AuthToken>.Error("User not found!.");
        }
        
        if (!await userManager.VerifyUserTokenAsync(user, jwtSettings.RefreshTokenProvider, "RefreshToken", command.RefreshToken))
        {
            return AppResponse<AuthToken>.Error("Refresh token expired");
        }

        return AppResponse<AuthToken>.Success(new()
        {
            RefreshToken = await TokenUtils.GenerateRefreshToken(userManager, jwtSettings, user),
            AccessToken = await TokenUtils.GenerateAccessToken(userManager, jwtSettings, user)
        });
    }
}