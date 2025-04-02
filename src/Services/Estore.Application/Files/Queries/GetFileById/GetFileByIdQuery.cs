using Estore.Application.Dtos.Files;

namespace Estore.Application.Files.Queries.GetFileById;

public record GetFileByIdQuery(Guid Id) : IQuery<AppResponse<FileInformationDto>>;

public class GetFileByIdQueryValidator : AbstractValidator<GetFileByIdQuery>
{
    public GetFileByIdQueryValidator()
    {
        RuleFor(x => x.Id)
                    .NotNull().WithMessage("Id is required.");
    }
}
