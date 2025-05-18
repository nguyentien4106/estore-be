using EStore.Domain.Enums.Files;
using EStore.Domain.Models.Base;

namespace EStore.Domain.Models;

public class TeleFileEntity : FileEntity
{
    public int? MessageId { get; set; }

    public long? FileId { get; set; }

    public long? AccessHash{get;set;}

    public uint? Flags {get; set;}

    public byte[]? FileReference { get; set; } = default!;

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int? DcId {get;set;}

    public string? Thumbnail {get;set;} = default!;

    public static TeleFileEntity Create(long fileId, long accessHash, uint flags, byte[] fileReference, int dc_id, int width, int height, string fileName, long fileSize, FileType fileType, string extension, string thumbnail, string userId, int messageId, string contentType)
    {
        return new TeleFileEntity{
            FileId = fileId,
            AccessHash = accessHash,
            Flags = flags,
            FileReference = fileReference,
            DcId = dc_id,
            Width = width,
            Height = height,
            FileName = fileName,
            FileSize = fileSize,
            FileType = fileType,
            Extension = extension,
            Thumbnail = thumbnail,
            UserId = userId,
            MessageId = messageId,
            ContentType = contentType
        };
    }

}