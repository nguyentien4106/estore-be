using EStore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EStore.Application.Data;

public interface IEStoreDbContext
{
    public DbSet<User> Users { get; }

    public DbSet<FileInformation> Files { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}