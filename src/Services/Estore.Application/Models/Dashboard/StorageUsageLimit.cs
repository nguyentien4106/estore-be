namespace EStore.Application.Models.Dashboard;

public class StorageUsageLimit
{
    public long MaxStorageSize { get; set; }
    public long RemainingStorage { get; set; }
    public double UsagePercentage { get; set; }
}