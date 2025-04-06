using EStore.Domain.Enums.Files;
using EStore.Domain.Models.Base;

namespace EStore.Domain.Models;

public class R2FileEntity : FileEntity
{
    public string FileKey{get;set;} = default!;

    public static R2FileEntity Create(string fileName, decimal fileSize, FileType fileType, string extension, string userId, string url, string key)
    {
        return new R2FileEntity{
            FileName = fileName,
            FileSize = fileSize,
            FileType = fileType,
            Extension = extension,
            UserId = userId,
            // Url = url,
            FileKey = key
        };
    }

}