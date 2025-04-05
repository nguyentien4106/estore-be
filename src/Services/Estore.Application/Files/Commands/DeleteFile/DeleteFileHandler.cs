using Estore.Application.Helpers;
using Estore.Application.Services;
using Estore.Application.Services.Files;
using Estore.Application.Services.Telegram;

namespace Estore.Application.Files.Commands.DeleteFile;

public class DeleteFileHandler(ITelegramService telegramService, IEStoreDbContext context) : ICommandHandler<DeleteFileCommand, AppResponse<Guid>>
{
    public async Task<AppResponse<Guid>> Handle(DeleteFileCommand command, CancellationToken cancellationToken)
    {
        // var deleteFileHandler = TelegramFileHandlerFactory.GetDeleteFileHandler(command.StorageSource, context, telegramService);
        //
        // return await deleteFileHandler.DeleteFileAsync(command.Id, cancellationToken);

        return AppResponse<Guid>.Error(null);
    }
}
