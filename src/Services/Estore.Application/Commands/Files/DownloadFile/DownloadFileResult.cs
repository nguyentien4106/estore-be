namespace EStore.Application.Commands.Files.DownloadFile;

public record DownloadFileResult(Stream FileStream, string FileName,string ContentType);