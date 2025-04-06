using EStore.Application.Dtos.Files;

namespace EStore.Application.Files.Queries.GetFileByName;

public record GetFileByNameQuery(string FileName) : IQuery<AppResponse<FileInformationDto>>;

public class GetFileByNameQueryValidator : AbstractValidator<GetFileByNameQuery>
{
    public GetFileByNameQueryValidator()
    {
        RuleFor(x => x.FileName)
                    .NotNull().WithMessage("FileName is required.");
    }
}
