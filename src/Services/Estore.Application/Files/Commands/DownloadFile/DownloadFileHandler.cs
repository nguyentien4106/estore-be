using Estore.Application.Helpers;
using Estore.Application.Services;
using Estore.Application.Services.Files;
using Estore.Application.Services.Telegram;

namespace Estore.Application.Files.Commands.DownloadFile;

public class DownloadFileHandler(IEStoreDbContext context) : ICommandHandler<DownloadFileCommand, AppResponse<DownloadFileResponse>>
{
    public async Task<AppResponse<DownloadFileResponse>> Handle(DownloadFileCommand command, CancellationToken cancellationToken)
    {
        // var deleteFileHandler = TelegramFileHandlerFactory.GetDeleteFileHandler(command.StorageSource, context, telegramService);
        //
        // return await deleteFileHandler.DeleteFileAsync(command.Id, cancellationToken);
        return AppResponse<DownloadFileResponse>.Error(null);
    }
}
