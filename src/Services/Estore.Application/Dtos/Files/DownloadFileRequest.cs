using Microsoft.AspNetCore.Http;

namespace Estore.Application.Dtos.Files;

public record DownloadFileRequest(Guid Id, StorageSource StorageSource);