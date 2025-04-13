using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using EStore.Domain.Abstractions;
using EStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EStore.Domain.Models;

namespace Estore.Infrastructure.Data.Interceptors
{
    public class FileInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? context)
        {
            if (context is null) return;

            // First, collect all the changes
            var changes = new List<(string UserId, long FileSize, EStore.Domain.Enums.Files.StorageSource StorageSource)>();

            foreach (var entry in context.ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is R2FileEntity r2File)
                    {
                        changes.Add((r2File.UserId, r2File.FileSize, EStore.Domain.Enums.Files.StorageSource.R2));
                    }

                    if (entry.Entity is TeleFileEntity teleFile)
                    {
                        changes.Add((teleFile.UserId, teleFile.FileSize, EStore.Domain.Enums.Files.StorageSource.Telegram));
                    }
                }

                if (entry.State == EntityState.Deleted)
                {
                    if (entry.Entity is R2FileEntity r2FileToDelete)
                    {
                        changes.Add((r2FileToDelete.UserId, -r2FileToDelete.FileSize, EStore.Domain.Enums.Files.StorageSource.R2));
                    }

                    if (entry.Entity is TeleFileEntity teleFileToDelete)
                    {
                        changes.Add((teleFileToDelete.UserId, -teleFileToDelete.FileSize, EStore.Domain.Enums.Files.StorageSource.Telegram));
                    }
                }
            }

            // Then apply all the changes
            foreach (var change in changes)
            {
                UpdateStorageUsage(context, change.UserId, change.FileSize, change.StorageSource);
            }
        }

        private void UpdateStorageUsage(DbContext context, string userId, long fileSize, EStore.Domain.Enums.Files.StorageSource storageSource)
        {
            var dbContext = context as EStoreDbContext;
            if (dbContext == null) return;
            
            var storageUsage = dbContext.StorageUsages
                .FirstOrDefault(su => su.UserId == userId && su.StorageSource == storageSource);
                
            if (storageUsage == null)
            {
                // Create new storage usage record if it doesn't exist
                storageUsage = new StorageUsage
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    UsedSize = fileSize,
                    StorageSource = storageSource
                };
                dbContext.StorageUsages.Add(storageUsage);
            }
            else
            {
                // Update existing storage usage
                storageUsage.UsedSize += fileSize;
                storageUsage.LastModified = DateTime.UtcNow;
                dbContext.StorageUsages.Update(storageUsage);
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}