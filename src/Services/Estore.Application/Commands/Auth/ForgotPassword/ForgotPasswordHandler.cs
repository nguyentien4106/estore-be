using EStore.Application.Exceptions;
using EStore.Domain.Models;
using BuildingBlocks.Auth.Models;
using Microsoft.AspNetCore.WebUtilities;


namespace EStore.Application.Commands.Auth.ForgotPassword;

public class ForgotPasswordHandler(UserManager<User> userManager, JwtSettings jwtSetting, IEmailSender<User> emailSender) : ICommandHandler<ForgotPasswordCommand, AppResponse<bool>>
{
    public async Task<AppResponse<bool>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email) ?? throw new UserNotFoundException(command.Email, command.Email);
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var url = $"{jwtSetting.Audience}/reset-password";
        var parameters = new Dictionary<string, string>()
        {
                { "token", token },
                { "email", command.Email },
        };

        var resetLink = new Uri(QueryHelpers.AddQueryString(url, parameters));
        await emailSender.SendPasswordResetLinkAsync(user, command.Email, resetLink.ToString());
        return AppResponse<bool>.Success(true);
    }
}
