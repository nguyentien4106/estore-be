using Estore.Domain.Enums.Files;
using EStore.Domain.Abstractions;

namespace EStore.Domain.Models;

public class FileInformation : Entity<Guid>
{
    public string FileName { get; set; } = default!;

    public decimal FileSize { get; set; }

    public FileType FileType { get; set; }

    public Guid UserId { get; set; }
}
