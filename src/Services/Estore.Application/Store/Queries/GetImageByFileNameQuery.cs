using Estore.Application.Constants;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Store.Commands;

public record GetImageByFileNameQuery(string FileName) : IQuery<AppResponse<string>>;

public class GetImageByFileNameQueryValidator : AbstractValidator<GetImageByFileNameQuery>
{
    public GetImageByFileNameQueryValidator()
    {
        RuleFor(x => x.FileName)
                    .NotNull().WithMessage("FileName is required.")
                    ;
    }
}
