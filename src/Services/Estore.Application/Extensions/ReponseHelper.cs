using EStore.Domain.Models.Base;

namespace EStore.Application.Extensions;

public static class ReponseHelper
{
    public static FileEntityResponse ToFileEntityResponse(this R2FileEntity file)
    {
        return new(file.Id.ToString(), file.FileName, file.FileSize, file.ContentType, StorageSource.R2, file.CreatedAt);
    }
    
    public static FileEntityResponse ToFileEntityResponse(this TeleFileEntity file)
    {
        return new(file.Id.ToString(), file.FileName, file.FileSize, file.ContentType, StorageSource.Telegram, file.CreatedAt);
    }
}