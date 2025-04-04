using TL;

namespace Estore.Application.Strategies.DownloadFiles;

public interface IDownloadFileHandler
{
    InputFileLocationBase GetLocation(TeleFileLocation fileLocation);
}