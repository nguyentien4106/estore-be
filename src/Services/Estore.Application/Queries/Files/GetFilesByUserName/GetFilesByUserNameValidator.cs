using FluentValidation;

namespace EStore.Application.Queries.Files.GetFilesByUserName;

public class GetFilesByUserNameQueryValidator : AbstractValidator<GetFilesByUserNameQuery>
{
    public GetFilesByUserNameQueryValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Request.PageIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Page index must be greater than or equal to 0.");

        RuleFor(x => x.Request.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");

        RuleFor(x => x.Request.StorageSource)
            .IsInEnum().WithMessage("Invalid storage source.");
    }
}
