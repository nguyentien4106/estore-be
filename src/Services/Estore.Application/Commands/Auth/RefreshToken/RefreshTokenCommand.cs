using Estore.Application.Models.Dtos;

namespace EStore.Application.Commands.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<AppResponse<AuthToken>>;