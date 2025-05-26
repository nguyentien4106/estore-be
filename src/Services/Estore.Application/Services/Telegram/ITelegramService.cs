using EStore.Application.Commands.Files.UploadFile;
using EStore.Application.Models.Files;

namespace EStore.Application.Services.Telegram;

public interface ITelegramService
{
    Task<AppResponse<TeleFileEntity>> UploadFileToStrorageAsync(UploadFileTelegramCommand command, string userId);

    Task<AppResponse<TeleFileEntity>> UploadFileToStrorageAsync(UploadFileHandlerArgs args, string userId);

    Task<AppResponse<Stream>> DownloadFileAsync(TeleFileEntity fileLocation);

    Task<AppResponse<bool>> DeleteMessageAsync(int messageId);

    Task<AppResponse<TeleFileEntity>> SendMessageAsync(UploadFileHandlerArgs args, string userId);

}
