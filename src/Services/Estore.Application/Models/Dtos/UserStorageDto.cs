namespace Estore.Application.Models.Dtos;

public record StorageUsageDto(string UserId, long UsedSize, StorageSource StorageSource);