namespace EStore.Application.Commands.Auth.Register;

public class RegisterAccountCommandValidator : AbstractValidator<RegisterAccountCommand>
{
    public RegisterAccountCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name cannot be empty");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name cannot be empty");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be empty");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty");
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username cannot be empty");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be empty").MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}