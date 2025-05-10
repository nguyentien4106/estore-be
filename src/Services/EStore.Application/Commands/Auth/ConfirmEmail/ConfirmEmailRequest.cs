using BuildingBlocks.Auth.Models;

namespace EStore.Application.Commands.Auth.ConfirmEmail;

public class ConfirmEmailRequest
{
    public string UserId { get; set; }
    public string Token { get; set; }
} 