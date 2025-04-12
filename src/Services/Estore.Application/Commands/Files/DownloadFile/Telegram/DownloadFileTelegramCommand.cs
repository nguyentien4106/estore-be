namespace EStore.Application.Commands.Files.DownloadFile;
public record DownloadFileTelegramCommand(Guid Id) : ICommand<AppResponse<DownloadFileResult>>;
