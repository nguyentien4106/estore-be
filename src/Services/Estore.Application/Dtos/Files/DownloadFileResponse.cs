using Microsoft.AspNetCore.Http;

namespace Estore.Application.Dtos.Files;

public record DownloadFileResponse(string FilePath, string ContentType);