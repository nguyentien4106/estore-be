namespace EStore.Application.Commands.Auth.ForgotPassword;

public class ForgotPasswordValidator: AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
    }
}

