using Estore.Domain.Enums.Files;

namespace EStore.Domain.Models;

public class TelegramFileInformation : FileInformation
{
    public string LocalPath { get; set; } = default!;

    public string? RemotePath { get; set; } = default!;

    public string? FileId { get; set; } = default!;

    public string? RemoteFileId { get; set;} = default!;

    public bool UploadCompleted { get; set; }

    public static TelegramFileInformation Create(Guid userId, string fileName, decimal fileSize, FileType fileType, string localPath, string fileId)
    {
        return new()
        {
            FileName = fileName,
            FileSize = fileSize,
            FileType = fileType,
            UserId = userId,
            LocalPath = localPath,
            FileId = fileId
        };
    }
}
