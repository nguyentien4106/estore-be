namespace EStore.Application.Models.Files;

public class ChunkMessage
{
    public long FileId { get; set; }
    public int ChunkIndex { get; set; }

    public long AccessHash { get; set; }

    public byte[] FileReference { get; set; }

}