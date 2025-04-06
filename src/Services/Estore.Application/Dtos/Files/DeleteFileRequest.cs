using Microsoft.AspNetCore.Http;

namespace EStore.Application.Dtos.Files;

public record DeleteFileRequest(Guid Id, StorageSource StorageSource);