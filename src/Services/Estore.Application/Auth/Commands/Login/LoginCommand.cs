using EStore.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using FluentValidation;

namespace EStore.Application.Auth.Commands.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand<AppResponse<EStoreToken>>;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}