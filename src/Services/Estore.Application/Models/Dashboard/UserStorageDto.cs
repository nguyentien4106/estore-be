namespace EStore.Application.Models.Dashboard;

public class UserStorageDto
{
    public string UserId { get; set; }
    public long TotalStorageUsed { get; set; }
    public long R2StorageUsed { get; set; }
    public long TelegramStorageUsed { get; set; }
    public int TotalFiles { get; set; }
    public int R2Files { get; set; }
    public int TelegramFiles { get; set; }
    public DateTime LastUpload { get; set; }
    public StorageUsageLimit StorageLimit { get; set; }
    public long TotalStorage { get; set; }
    public long UsedStorage { get; set; }
    public long AvailableStorage => TotalStorage - UsedStorage;
    public double UsagePercentage => TotalStorage > 0 ? (double)UsedStorage / TotalStorage * 100 : 0;
    public UserSubscriptionDto Subscription { get; set; } = new();
}