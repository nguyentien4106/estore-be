
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
}