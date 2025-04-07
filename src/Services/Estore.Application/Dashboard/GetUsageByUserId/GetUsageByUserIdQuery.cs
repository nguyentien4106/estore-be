using Estore.Application.Dtos.Dashboard;

namespace Estore.Application.Dashboard.GetUsageByUserId;

public record GetUsageByUserIdQuery(string UserId) : IQuery<AppResponse<List<StorageUsageDto>>>;
    