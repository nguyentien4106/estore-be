using EStore.Domain.Abstractions;
using EStore.Domain.Enums.Files;

namespace EStore.Domain.Models;

public class StorageUsage : Entity<Guid>
{
    public string UserId { get; set; } = default!;

    public long UsedSize { get; set; }

    public StorageSource StorageSource { get; set; }

}