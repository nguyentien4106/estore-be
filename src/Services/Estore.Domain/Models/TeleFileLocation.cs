using EStore.Domain.Abstractions;

namespace EStore.Domain.Models;

public class TeleFileLocation : Entity<Guid>
{
    public long FileId { get; set; }

    public long AccessHash{get;set;}

    public string ThumbSize{get;set;}

    public byte[] FileReference{get;set;}

    public static TeleFileLocation Create(long id, long accessHash, string thumbSize, byte[] fileReference )
    {
        return new TeleFileLocation{
            FileId = id,
            AccessHash = accessHash,
            ThumbSize = thumbSize,
            FileReference = fileReference
        };
    }
}