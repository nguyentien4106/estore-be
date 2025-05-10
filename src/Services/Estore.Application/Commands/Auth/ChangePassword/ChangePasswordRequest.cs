using BuildingBlocks.Auth.Models;

namespace EStore.Application.Commands.Auth.ChangePassword;

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
} 