namespace EStore.Application.Commands.Files.DeleteFile;

public record DeleteFileTelegramCommand(Guid Id) : ICommand<AppResponse<Guid>>;
