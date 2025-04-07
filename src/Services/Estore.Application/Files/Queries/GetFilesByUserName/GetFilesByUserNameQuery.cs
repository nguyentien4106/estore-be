using BuildingBlocks.Pagination;

namespace EStore.Application.Files.Queries.GetFilesByUserName;

public record GetFilesByUserNameQuery(string UserName, PaginationRequest PaginationRequest) : IQuery<AppResponse<PaginatedResult<FileEntityResponse>>>;

public class GetFilesByUserNameQueryValidator : AbstractValidator<GetFilesByUserNameQuery>
{
    public GetFilesByUserNameQueryValidator()
    {
            RuleFor(x => x.UserName)
                    .NotNull().WithMessage("UserName is required.");
        }
}
