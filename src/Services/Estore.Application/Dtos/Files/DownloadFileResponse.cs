using Microsoft.AspNetCore.Http;

namespace EStore.Application.Dtos.Files;

public record DownloadFileResponse(string FilePath, string ContentType);