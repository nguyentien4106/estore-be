namespace EStore.Application.Auth.Commands.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<AppResponse<AuthToken>>;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("RefreshToken is required");
    }
}