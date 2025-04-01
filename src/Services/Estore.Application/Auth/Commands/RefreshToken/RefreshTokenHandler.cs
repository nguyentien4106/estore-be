using EStore.Application.Helpers;
using EStore.Domain.Models;
using BuildingBlocks.Auth.Constants;
using BuildingBlocks.Auth.Models;
using EStore.Application.Data;

namespace EStore.Application.Auth.Commands.Auth.RefreshToken;

public class RefreshTokenHandler(JwtSettings jwtSettings, UserManager<User> userManager, IEStoreDbContext context) : ICommandHandler<RefreshTokenCommand, AppResponse<AuthToken>>
{
    public async Task<AppResponse<AuthToken>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        // Validate and decode the refresh token
        
        var validationResult = await TokenUtils.Ge(userManager, jwtSettings, command.RefreshToken);
        if (!validationResult.IsValid)
        {
            return AppResponse<AuthToken>.Error("Invalid or expired refresh token");
        }

        var user = validationResult.User;
        
        // Generate new tokens
        return AppResponse<AuthToken>.Success(new()
        {
            RefreshToken = await TokenUtils.GenerateRefreshToken(userManager, jwtSettings, user),
            AccessToken = await TokenUtils.GenerateAccessToken(userManager, jwtSettings, user)
        });
    }
}