using EStore.Application.Models.Files;
using Microsoft.AspNetCore.Http;

namespace EStore.Application.Commands.Files.UploadFileMultipart;

public record UploadFileMultipartCommand(
    IFormFile File,
    int ChunkIndex,
    int TotalChunks,
    string FileName,
    string UserId,
    string FileId,
    string UserName,
    string ContentType
) : ICommand<AppResponse<FileEntityResult>>;
