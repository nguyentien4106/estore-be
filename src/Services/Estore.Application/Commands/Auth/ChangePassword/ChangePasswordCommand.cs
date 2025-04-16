using BuildingBlocks.Auth.Models;

namespace EStore.Application.Commands.Auth.ChangePassword;

public record ChangePasswordCommand(string UserName, string CurrentPassword, string NewPassword, string ConfirmPassword) : ICommand<AppResponse<bool>>;