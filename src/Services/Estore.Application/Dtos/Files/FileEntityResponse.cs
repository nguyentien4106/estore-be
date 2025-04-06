namespace EStore.Application.Dtos.Files;

public record FileEntityResponse(string Id, string FileName, decimal FileSize, string ContentType, StorageSource StorageSource, DateTime? CreatedAt);