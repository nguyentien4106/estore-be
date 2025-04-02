using Estore.Domain.Enums.Files;
using EStore.Domain.Abstractions;

namespace EStore.Domain.Models;

public class FileInformation : Entity<Guid>
{
    public string FileName { get; set; }

    public string StorageFileName { get; set; }

    public decimal FileSize { get; set; }

    public string Url { get; set; }

    public FileType FileType { get; set; }

    public StorageSource StorageSource { get; set; }

    public Guid UserId { get; set; }

    public static FileInformation Create(Guid userId, string fileName, decimal fileSize, 
        string url, FileType fileType, StorageSource storageSource, string storageFileName)
    {
        return new()
        {
            FileName = fileName,
            FileSize = fileSize,
            Url = url,
            FileType = fileType,
            StorageSource = storageSource,
            UserId = userId,
            StorageFileName = storageFileName
        };
    }

}
