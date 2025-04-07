using Estore.Application.Dtos.Dashboard;

namespace Estore.Application.Dashboard.GetUsageByUserId
{
    public class GetUsageByUserIdHandler(IEStoreDbContext context) : IQueryHandler<GetUsageByUserIdQuery, AppResponse<List<StorageUsageDto>>>
    {
        public async Task<AppResponse<List<StorageUsageDto>>> Handle(GetUsageByUserIdQuery request, CancellationToken cancellationToken)
        {
            var r2Usage = await context.StorageUsages.Where(x => x.UserId == request.UserId && x.StorageSource == StorageSource.R2).FirstOrDefaultAsync(cancellationToken);
            var teleUsage = await context.StorageUsages.Where(x => x.UserId == request.UserId && x.StorageSource == StorageSource.Telegram).FirstOrDefaultAsync(cancellationToken);
            
            return AppResponse<List<StorageUsageDto>>.Success(new List<StorageUsageDto>
            {
                new StorageUsageDto(request.UserId, r2Usage?.UsedSize ?? 0, StorageSource.R2),
                new StorageUsageDto(request.UserId, teleUsage?.UsedSize ?? 0, StorageSource.Telegram)
            });
        }
    }
}