namespace EStore.Application.Commands.Files.DownloadFile;
public record DownloadFileR2Command(Guid Id) : ICommand<AppResponse<DownloadFileResult>>;