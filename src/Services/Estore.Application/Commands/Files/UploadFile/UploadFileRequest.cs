using Microsoft.AspNetCore.Http;

namespace EStore.Application.Commands.Files.UploadFile;

public record UploadFileRequest(IFormFile File, string UserName, StorageSource StorageSource);