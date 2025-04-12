namespace EStore.Application.Commands.Files.DownloadFile;

public record DownloadFileRequest(Guid Id, StorageSource StorageSource);