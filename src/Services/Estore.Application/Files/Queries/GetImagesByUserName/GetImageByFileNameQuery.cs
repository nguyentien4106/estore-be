namespace Estore.Application.Files.Queries.GetImagesByUserName;

public record GetImagesByUserNameQuery(string UserName) : IQuery<AppResponse<List<R2File>>>;

public class GetImagesByUserNameQueryValidator : AbstractValidator<GetImagesByUserNameQuery>
{
    public GetImagesByUserNameQueryValidator()
    {
            RuleFor(x => x.UserName)
                    .NotNull().WithMessage("UserName is required.");
    }
}
