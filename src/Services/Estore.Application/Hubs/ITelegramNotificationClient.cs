using EStore.Application.Models.Files;
using System.Threading.Tasks;

namespace EStore.Application.Hubs
{
    public interface ITelegramNotificationClient
    {
        Task ReceiveUploadProgress(string fileId, double percentage);
        Task ReceiveUploadCompleted(string fileId, FileEntityResult? result);

        Task ReceiveDownloadStarted(string fileId, string fileName);
        Task ReceiveDownloadProgress(string fileId, string fileName, double percentage);
    }
} 