using Microsoft.AspNetCore.Http;

namespace EStore.Application.Dtos.Files;

public record UploadFileRequest(IFormFile File, string UserName, StorageSource StorageSource);