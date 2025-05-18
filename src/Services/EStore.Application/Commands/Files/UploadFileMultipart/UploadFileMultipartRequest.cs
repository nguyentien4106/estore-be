using Microsoft.AspNetCore.Http;

namespace EStore.Application.Commands.Files.UploadFileMultipart;

public record UploadFileMultipartRequest(
    IFormFile File,
    int ChunkIndex,
    int TotalChunks,
    string FileName,
    string UserId,
    string FileId,
    string UserName
);
