namespace EStore.Application.Commands.Auth.RefreshToken;


public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand
>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("RefreshToken is required");
    }
}