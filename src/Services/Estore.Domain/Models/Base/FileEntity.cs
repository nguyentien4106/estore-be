using EStore.Domain.Enums.Files;
using EStore.Domain.Abstractions;

namespace EStore.Domain.Models.Base;

public class FileEntity : Entity<Guid>
{
    public string FileName { get; set; } = default!;
    public decimal FileSize { get; set; }
    public FileType FileType { get; set; }
    public string Extension { get; set; } = default!;
    public string UserId { get; set; }
    public string ContentType { get; set; } = default!;
}