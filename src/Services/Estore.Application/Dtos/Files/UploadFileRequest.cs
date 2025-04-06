using Microsoft.AspNetCore.Http;

namespace EStore.Application.Dtos.Files;

public record UploadFileRequest(IFormFile File, string UserName, int Width, int Height, StorageSource StorageSource);