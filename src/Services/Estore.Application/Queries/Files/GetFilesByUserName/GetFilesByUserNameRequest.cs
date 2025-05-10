using EStore.Domain.Enums.Files;

namespace EStore.Application.Queries.Files.GetFilesByUserName;

public record GetFilesByUserNameRequest
{
    public int PageIndex { get; init; } = 0;
    public int PageSize { get; init; } = 10;
    public StorageSource StorageSource { get; init; } = StorageSource.R2;
}