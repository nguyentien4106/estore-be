using EStore.Application.Models.Dashboard;

namespace EStore.Application.Queries.Dashboard;

public class GetUserStorageQueryHandler(IEStoreDbContext _context) : IQueryHandler<GetUserStorageQuery, AppResponse<UserStorageDto>>
{
    public async Task<AppResponse<UserStorageDto>> Handle(GetUserStorageQuery query, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == query.UserName, cancellationToken);

        if (user == null)
        {
            return AppResponse<UserStorageDto>.Error("User not found");
        }
        
        var storageUsage = await _context.StorageUsages
            .Where(su => su.UserId == user.Id)
            .GroupBy(su => su.StorageSource)
            .Select(g => new
            {
                Source = g.Key,
                UsedSize = g.Sum(x => x.UsedSize)
            })
            .ToListAsync(cancellationToken);

        var r2Files = await _context.R2FileEntities
            .Where(f => f.UserId == user.Id)
            .CountAsync(cancellationToken);

        var teleFiles = await _context.TeleFileEntities
            .Where(f => f.UserId == user.Id)
            .CountAsync(cancellationToken);


        var r2Storage = storageUsage
            .FirstOrDefault(x => x.Source == StorageSource.R2)?.UsedSize ?? 0;
        var teleStorage = storageUsage
            .FirstOrDefault(x => x.Source == StorageSource.Telegram)?.UsedSize ?? 0;

        const long maxStorage = 1024 * 1024 * 1024; // 1GB example limit
        var totalUsed = r2Storage + teleStorage;
        
        var result = new UserStorageDto
        {
            UserId = user.Id,
            TotalStorageUsed = totalUsed,
            R2StorageUsed = r2Storage,
            TelegramStorageUsed = teleStorage,
            TotalFiles = r2Files + teleFiles,
            R2Files = r2Files,
            TelegramFiles = teleFiles,
            LastUpload = DateTime.MinValue,
            StorageLimit = new StorageUsageLimit
            {
                MaxStorageSize = maxStorage,
                RemainingStorage = maxStorage - totalUsed,
                UsagePercentage = (double)totalUsed / maxStorage * 100
            }
        };

        return AppResponse<UserStorageDto>.Success(result);
    }
} 