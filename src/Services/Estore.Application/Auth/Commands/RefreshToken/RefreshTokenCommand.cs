namespace EStore.Application.Auth.Commands.Auth.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<AppResponse<EStoreToken>>;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty().WithMessage("AccessToken is required");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("RefreshToken is required");
    }
}