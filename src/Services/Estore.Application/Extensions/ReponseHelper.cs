using EStore.Application.Models.Files;

namespace EStore.Application.Extensions;

public static class ReponseHelper
{
    public static FileEntityResult ToFileEntityResponse(this R2FileEntity file)
    {
        return new(file.Id.ToString(), file.FileName, file.FileSize, file.ContentType, StorageSource.R2, file.CreatedAt, file.FileStatus);
    }
    
    public static FileEntityResult ToFileEntityResponse(this TeleFileEntity file)
    {
        return new(file.Id.ToString(), file.FileName, file.FileSize, file.ContentType, StorageSource.Telegram, file.CreatedAt, file.FileStatus);
    }
}