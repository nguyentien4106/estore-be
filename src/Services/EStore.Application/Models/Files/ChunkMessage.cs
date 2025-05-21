namespace EStore.Application.Models.Files;

public class ChunkMessage
{
    public Guid Id { get; set; } = Guid.Empty;
    public string FileId { get; set; }
    public string UserId { get; set; }
    public string FilePath { get; set; }
    public int ChunkIndex { get; set; }
    public int TotalChunks { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }

}