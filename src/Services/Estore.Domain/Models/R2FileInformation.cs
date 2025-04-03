using Estore.Domain.Enums.Files;
using EStore.Domain.Abstractions;

namespace EStore.Domain.Models;

public class R2FileInformation : FileInformation
{
    public string StorageFileName { get; set; } = default!;

    public string Url { get; set; } = default!;

    public static R2FileInformation Create(Guid userId, string fileName, decimal fileSize, 
        string url, FileType fileType, string storageFileName)
    {
        return new()
        {
            FileName = fileName,
            FileSize = fileSize,
            Url = url,
            FileType = fileType,
            UserId = userId,
            StorageFileName = storageFileName
        };
    }

}
