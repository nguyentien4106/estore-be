using BuildingBlocks.Auth.Models;

namespace EStore.Application.Commands.Auth.ConfirmEmail;

public class ConfirmEmailCommand : ICommand<AppResponse<bool>>
{
    public string UserId { get; set; }
    public string Token { get; set; }
} 