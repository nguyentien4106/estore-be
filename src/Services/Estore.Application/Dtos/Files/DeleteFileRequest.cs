using Microsoft.AspNetCore.Http;

namespace Estore.Application.Dtos.Files;

public record DeleteFileRequest(Guid Id, StorageSource StorageSource);