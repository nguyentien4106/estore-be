using EStore.Application.Files.Commands.DeleteFileR2;
using EStore.Application.Files.Commands.DeleteFileTelegram;
using EStore.Application.Files.Commands.DownloadFileR2;
using EStore.Application.Files.Commands.DownloadFileTelegram;
using EStore.Application.Files.Commands.UploadFileR2;
using EStore.Application.Files.Commands.UploadFileTelegram;
using EStore.Domain.Models.Base;

namespace EStore.Application.Factories;

public class CommandHandlerFactory
{
    public static ICommand<AppResponse<FileEntityResponse>> GetUploadFileCommand(UploadFileRequest request)
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

    public static ICommand<AppResponse<DownloadFileResponse>> GetDownloadFileCommand(DownloadFileRequest request)
    {
        return request.StorageSource switch
        {
            StorageSource.R2 => new DownloadFileR2Command(request.Id),
            StorageSource.Telegram => new DownloadFileTelegramCommand(request.Id),
            _ => throw new NotSupportedException($"Storage source {request.StorageSource} is not supported")
        };
    }   

}
