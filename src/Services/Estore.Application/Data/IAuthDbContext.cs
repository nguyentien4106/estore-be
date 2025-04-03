using EStore.Domain.Models;

namespace EStore.Application.Data;

public interface IEStoreDbContext
{
    public DbSet<User> Users { get; }

    public DbSet<R2FileInformation> R2Files { get; }

    public DbSet<TelegramFileInformation> TeleFiles { get; }
    
    public DbSet<TeleFileLocation> TeleFilesLocations { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}