using Microsoft.AspNetCore.Http;

namespace EStore.Application.Dtos.Files;

public record DownloadFileResponse(Stream FileStream, string FileName,string ContentType);