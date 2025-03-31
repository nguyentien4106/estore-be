using System.Security.Claims;
using System.Text;
using EStore.Application.Exceptions;
using EStore.Application.Helpers;
using EStore.Domain.Models;
using EStore.Domain.ValueObjects;
using BuildingBlocks.Auth.Models;
using BuildingBlocks.CQRS;
using BuildingBlocks.Models;

namespace EStore.Application.Auth.Commands.Auth.Login;

public class LoginHandler(SignInManager<User> signInManager, UserManager<User> userManager, JwtSettings jwtSettings) : ICommandHandler<LoginCommand, AppResponse<EStoreToken>>
{
    public async Task<AppResponse<EStoreToken>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = userManager.FindByEmailAsync(command.Email).Result;

        if (user is null)
        {
            throw new UserNotFoundException("User", command.Email);
        }
        
        var result = await signInManager.PasswordSignInAsync(user, command.Password, true, true);
        if (result.Succeeded)
        {
            var token = await GenerateUserToken(user);
            return AppResponse<EStoreToken>.Success(token);
        }

        if (result.IsLockedOut) 
        {
            return AppResponse<EStoreToken>.Error("Your account has been locked out");
        }

        if (result.IsNotAllowed)
        {
            return AppResponse<EStoreToken>.Error("Your account hasn't been allowed");
        }

        if (result.RequiresTwoFactor)
        {
            return AppResponse<EStoreToken>.Error("Your account didn't turn the two factor on! Please turn 2FA on first.");
        }

        return AppResponse<EStoreToken>.Error("Password was incorrect! Please try again.");
    }

    private async Task<EStoreToken> GenerateUserToken(User user)
    {
       
        var accessToken = await TokenUtils.GenerateAccessToken(userManager, jwtSettings, user);
        var refreshToken =  await TokenUtils.GenerateRefreshToken(userManager, jwtSettings, user);
        
        return new EStoreToken
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}