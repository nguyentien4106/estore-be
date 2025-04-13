namespace EStore.Application.Queries.Dashboard;

public class GetUserStorageQueryValidator : AbstractValidator<GetUserStorageQuery>
{
    public GetUserStorageQueryValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("UserName is required");
    }
} 