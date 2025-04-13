namespace EStore.Application.Commands.Files.DeleteFile;

public record DeleteFileRequest(Guid Id, StorageSource StorageSource);