using EStore.Domain.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EStore.Application.Data;

public interface IEStoreDbContext
{
    public DbSet<User> Users { get; }

    public DbSet<TeleFileEntity> TeleFileEntities { get; }

    public DbSet<R2FileEntity> R2FileEntities { get; }

    public DbSet<StorageUsage> StorageUsages { get; }

    public DbSet<Order> Orders { get; }

    public DbSet<Payment> Payments { get; }

    public DatabaseFacade Database { get; }
    
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}