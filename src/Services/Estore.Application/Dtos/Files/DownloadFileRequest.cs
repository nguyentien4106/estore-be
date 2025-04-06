using Microsoft.AspNetCore.Http;

namespace EStore.Application.Dtos.Files;

public record DownloadFileRequest(Guid Id, StorageSource StorageSource);