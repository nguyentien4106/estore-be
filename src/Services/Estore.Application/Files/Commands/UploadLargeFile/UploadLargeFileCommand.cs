using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace EStore.Application.Files.Commands.UploadLargeFile;

public record UploadLargeFileCommand(HttpRequest Request, HttpResponse Response, MediaTypeHeaderValue? Headers) : ICommand<AppResponse<FileEntityResponse>>;

