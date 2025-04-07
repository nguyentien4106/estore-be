namespace Estore.Application.Dtos.Dashboard;

public record StorageUsageDto(string UserId, long UsedSize, StorageSource StorageSource);