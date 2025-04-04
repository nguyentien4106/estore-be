using Estore.Domain.Enums.Files;
using EStore.Domain.Abstractions;

namespace EStore.Domain.Models;

public class TeleFileLocation : Entity<Guid>
{
    
    public long FileId { get; set; }

    public long AccessHash{get;set;}

    public uint Flags {get; set;}

    public string FileName { get; set; } = default!;

    public decimal FileSize { get; set; }

    public FileType FileType { get; set; }

    public string Extension { get; set; } = default!;

    public byte[] FileReference { get; set; } = default!;

    public string UserId { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int DcId {get;set;}

    public string Thumbnail {get;set;} = default!;

    public static TeleFileLocation Create(long fileId, long accessHash, uint flags, byte[] fileReference, int dc_id, int width, int height, string fileName, decimal fileSize, FileType fileType, string extension, string thumbnail, string userId)
    {
        return new TeleFileLocation{
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
            UserId = userId
        };
    }

}