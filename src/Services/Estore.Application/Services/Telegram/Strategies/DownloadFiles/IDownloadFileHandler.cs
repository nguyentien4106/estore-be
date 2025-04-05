using TL;

namespace Estore.Application.Services.Telegram.Strategies.DownloadFiles;

public interface IDownloadFileHandler
{
    InputFileLocationBase GetLocation(TeleFileEntity fileLocation);
}