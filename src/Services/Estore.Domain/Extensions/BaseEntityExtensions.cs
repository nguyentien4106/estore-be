using EStore.Domain.Models;
using EStore.Domain.Models.Base;

namespace EStore.Domain.Extensions;

public static class BaseEntityExtensions
{
    public static R2FileEntity ToR2FileEntity(this FileEntity entity)
    {
        return new R2FileEntity
        {
            FileName = entity.FileName,
            FileSize = entity.FileSize,
            FileType = entity.FileType,
            Extension = entity.Extension,
            ContentType = entity.ContentType,
            UserId = entity.UserId,
            CreatedAt = entity.CreatedAt,
            Id = entity.Id
        };
    }
    
}