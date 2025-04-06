using TL;

namespace EStore.Application.Services.Telegram.Strategies.DownloadFiles;

public interface IDownloadFileHandler
{
    InputFileLocationBase GetLocation(TeleFileEntity fileLocation);
}