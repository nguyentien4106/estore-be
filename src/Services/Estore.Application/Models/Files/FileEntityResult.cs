namespace EStore.Application.Models.Files;

public record FileEntityResult(string Id, string FileName, decimal FileSize, string ContentType, StorageSource StorageSource, DateTime? CreatedAt, FileStatus FileStatus);