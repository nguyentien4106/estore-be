using EStore.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EStore.Application.Data;

public interface IEStoreDbContext
{
    public DbSet<User> Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}