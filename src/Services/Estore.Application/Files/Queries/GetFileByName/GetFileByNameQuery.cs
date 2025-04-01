namespace Estore.Application.Files.Queries.GetFileByName;

public record GetFileByNameQuery(string FileName) : IQuery<AppResponse<R2File>>;

public class GetFileByNameQueryValidator : AbstractValidator<GetFileByNameQuery>
{
    public GetFileByNameQueryValidator()
    {
        RuleFor(x => x.FileName)
                    .NotNull().WithMessage("FileName is required.");
    }
}
