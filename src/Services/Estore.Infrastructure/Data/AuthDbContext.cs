using System.Reflection;
using EStore.Application.Data;
using EStore.Domain.Models;
using Microsoft.AspNetCore.Identity;
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
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=EStoreDb;User Id=sa;Password=SwN12345678;Encrypt=False;TrustServerCertificate=True");

    } 


    public DbSet<User> ApplicationUsers => Users ;
}