using EStore.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;

namespace EStore.Application.Commands.Auth.ConfirmEmail;

public class ConfirmEmailHandler : ICommandHandler<ConfirmEmailCommand, AppResponse<bool>>
{
    private readonly UserManager<User> _userManager;

    public ConfirmEmailHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AppResponse<bool>> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.UserId) || string.IsNullOrEmpty(command.Token))
        {
            return AppResponse<bool>.Error("User ID and Token are required.");
        }

        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null)
        {
            return AppResponse<bool>.NotFound("User", command.UserId);
        }

        var result = await _userManager.ConfirmEmailAsync(user, command.Token);

        if (result.Succeeded)
        {
            user.EmailConfirmed = true;
            user.Status = (int)AccountStatus.Active;
            await _userManager.UpdateAsync(user); // Ensure changes are saved
            return AppResponse<bool>.Success(true, "Email confirmed successfully.");
        }

        var errors = result.Errors.Select(e => e.Description).ToList();
        return AppResponse<bool>.Error(errors.FirstOrDefault() ?? "Email confirmation failed.");
    }
} 