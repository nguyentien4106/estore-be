using Estore.Domain.Enums.Files;
using EStore.Domain.Abstractions;

namespace EStore.Domain.Models.Bases;

public class FileEntity : Entity<Guid>
{
    public string FileName { get; set; } = default!;

    public decimal FileSize { get; set; }

    public FileType FileType { get; set; }

    public string Extension { get; set; } = default!;

    public byte[] FileReference { get; set; } = default!;

    public Guid UserId { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int DcId {get;set;}

    public long FileId { get; set; }

    public long AccessHash{get;set;}

    public uint Flags {get; set;}

}
