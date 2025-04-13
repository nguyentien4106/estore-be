using Estore.Application.Models.Dtos;

namespace EStore.Application.Commands.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand<AppResponse<AuthToken>>;