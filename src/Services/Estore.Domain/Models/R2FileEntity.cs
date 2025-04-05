using Estore.Domain.Enums.Files;
using Estore.Domain.Models.Base;

namespace EStore.Domain.Models;

public class R2FileEntity : FileEntity
{
    public string Url{get;set;} = default!;

    public string BucketFileName{get;set;} = default!;

    public static R2FileEntity Create(string fileName, decimal fileSize, FileType fileType, string extension, string userId, string url, string bucketFileName)
    {
        return new R2FileEntity{
            FileName = fileName,
            FileSize = fileSize,
            FileType = fileType,
            Extension = extension,
            UserId = userId,
            Url = url,
            BucketFileName = bucketFileName
        };
    }

}