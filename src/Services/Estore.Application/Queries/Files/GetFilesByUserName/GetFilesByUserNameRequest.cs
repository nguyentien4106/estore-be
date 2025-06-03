using BuildingBlocks.Pagination;
using EStore.Domain.Enums.Files;

namespace EStore.Application.Queries.Files.GetFilesByUserName;

public class GetFilesByUserNameRequest : PaginationRequest
{
    public StorageSource StorageSource { get; init; } = StorageSource.R2;
}