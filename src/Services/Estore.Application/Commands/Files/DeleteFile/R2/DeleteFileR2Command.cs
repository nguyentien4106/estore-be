namespace EStore.Application.Commands.Files.DeleteFile;

public record DeleteFileR2Command(Guid Id) : ICommand<AppResponse<Guid>>;

