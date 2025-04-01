namespace Estore.Application.Files.Queries.GetImageByFileName;

public record GetImageByFileNameQuery(string FileName) : IQuery<AppResponse<R2File>>;

public class GetImageByFileNameQueryValidator : AbstractValidator<GetImageByFileNameQuery>
{
    public GetImageByFileNameQueryValidator()
    {
        RuleFor(x => x.FileName)
                    .NotNull().WithMessage("FileName is required.");
    }
}
