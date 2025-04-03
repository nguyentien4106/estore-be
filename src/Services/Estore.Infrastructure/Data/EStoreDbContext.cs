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

    public DbSet<R2FileInformation> R2FileInfos { get; set; }

    public DbSet<TelegramFileInformation> TeleFileInfos { get; set; }
    
    public DbSet<TeleFileLocation> TeleFileLocations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=EStoreDb;User Id=postgres;Password=postgres");
    }

    public new DbSet<User> Users => base.Users;

    public DbSet<R2FileInformation> R2Files => R2FileInfos;

    public DbSet<TelegramFileInformation> TeleFiles => TeleFileInfos;

    public DbSet<TeleFileLocation> TeleFilesLocations => TeleFileLocations;
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}