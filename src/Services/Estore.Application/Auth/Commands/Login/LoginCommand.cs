namespace EStore.Application.Auth.Commands.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand<AppResponse<AuthToken>>;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}