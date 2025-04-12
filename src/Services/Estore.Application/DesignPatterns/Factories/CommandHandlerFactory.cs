using EStore.Application.Commands.Files.DeleteFile;
using EStore.Application.Commands.Files.DownloadFile;
using EStore.Application.Commands.Files.UploadFile;
using EStore.Application.Models.Files;

namespace EStore.Application.DesignPatterns.Factories;

public class CommandHandlerFactory
{
    public static ICommand<AppResponse<FileEntityResult>> GetUploadFileCommand(UploadFileRequest request)
    {
        return request.StorageSource switch
        {
            StorageSource.R2 => new UploadFileR2Command(request.File, request.UserName),
            StorageSource.Telegram => new UploadFileTelegramCommand(request.File, request.UserName),
            _ => throw new NotSupportedException($"Storage source {request.StorageSource} is not supported")
        };
    }

    public static ICommand<AppResponse<Guid>> GetDeleteFileCommand(DeleteFileRequest request)
    {
        return request.StorageSource switch
        {
            StorageSource.R2 => new DeleteFileR2Command(request.Id),
            StorageSource.Telegram => new DeleteFileTelegramCommand(request.Id),
            _ => throw new NotSupportedException($"Storage source {request.StorageSource} is not supported")
        };
    }

    public static ICommand<AppResponse<DownloadFileResult>> GetDownloadFileCommand(DownloadFileRequest request)
    {
        return request.StorageSource switch
        {
            StorageSource.R2 => new DownloadFileR2Command(request.Id),
            StorageSource.Telegram => new DownloadFileTelegramCommand(request.Id),
            _ => throw new NotSupportedException($"Storage source {request.StorageSource} is not supported")
        };
    }   

}
