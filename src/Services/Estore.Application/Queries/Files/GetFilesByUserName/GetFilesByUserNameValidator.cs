namespace EStore.Application.Queries.Files.GetFilesByUserName;

public class GetFilesByUserNameQueryValidator : AbstractValidator<GetFilesByUserNameQuery>
{
    public GetFilesByUserNameQueryValidator()
    {
            RuleFor(x => x.UserName)
                    .NotNull().WithMessage("UserName is required.");
        }
}
