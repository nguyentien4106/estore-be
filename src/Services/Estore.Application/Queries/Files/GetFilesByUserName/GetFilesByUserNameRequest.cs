using BuildingBlocks.Pagination;

namespace EStore.Application.Queries.Files.GetFilesByUserName;

public record GetFilesByUserNameRequest
{
    public string UserName { get; init; }
    public int PageIndex { get; init; } = 0;
    public int PageSize { get; init; } = 10;
}