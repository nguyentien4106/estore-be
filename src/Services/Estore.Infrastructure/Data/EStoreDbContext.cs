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

    // public DbSet<R2FileInformation> R2FileInfos { get; set; }
    //
    // public DbSet<TelegramFileInformation> TeleFileInfos { get; set; }
    
    public DbSet<TeleFileEntity> TeleFileEntities { get; set; }

    public DbSet<R2FileEntity> R2FileEntities { get; set; }

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
    //
    // public DbSet<R2FileInformation> R2Files => R2FileInfos;
    //
    // public DbSet<TelegramFileInformation> TeleFiles => TeleFileInfos;

    // public DbSet<TeleFileEntity> TeleFileEntities => this.TeleFileEntities;

    // public DbSet<R2FileEntity> R2FileEntities => this.R2FileEntities;
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}