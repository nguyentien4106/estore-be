using System.Reflection;
using EStore.Application.Data;
using EStore.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EStore.Infrastructure.Data;

public class EStoreDbContext: IdentityDbContext<User>, IEStoreDbContext
{
    public EStoreDbContext()
    {
    }

    public EStoreDbContext(DbContextOptions<EStoreDbContext> options) : base(options)
    {
    }

    public new DbSet<User> Users => base.Users;
    
    public DbSet<TeleFileEntity> TeleFileEntities { get; set; }

    public DbSet<R2FileEntity> R2FileEntities { get; set; }

    public DbSet<StorageUsage> StorageUsages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=EStoreDb;User Id=postgres;Password=postgres");
    }

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}