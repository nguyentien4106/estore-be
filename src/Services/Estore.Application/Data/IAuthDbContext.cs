using EStore.Domain.Models;

namespace EStore.Application.Data;

public interface IEStoreDbContext
{
    public DbSet<User> Users { get; }

    public DbSet<TeleFileEntity> TeleFileEntities { get; }

    public DbSet<R2FileEntity> R2FileEntities { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}