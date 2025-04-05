using Estore.Application.Files.Commands.DeleteFile;
using Estore.Application.Files.Commands.DeleteFileTelegram;
using Estore.Application.Files.Commands.DownloadFile;
using Estore.Application.Files.Commands.DownloadFileTelegram;
using Estore.Application.Files.Commands.UploadFile;
using Estore.Application.Files.Commands.UploadFileTelegram;
using Estore.Domain.Models.Base;

namespace Estore.Application.Factories;

public class CommandHandlerFactory
{
    public static ICommand<AppResponse<FileEntity>> GetUploadFileCommand(UploadFileRequest request)
    {
        return request.StorageSource switch
        {
            StorageSource.R2 => new UploadFileCommand(request.File, request.UserName, request.Width, request.Height),
            StorageSource.Telegram => new UploadFileTelegramCommand(request.File, request.UserName, request.Width, request.Height),
            _ => throw new NotSupportedException($"Storage source {request.StorageSource} is not supported")
        };
    }

    public static ICommand<AppResponse<Guid>> GetDeleteFileCommand(DeleteFileRequest request)
    {
        return request.StorageSource switch
        {
            StorageSource.R2 => new DeleteFileCommand(request.Id),
            StorageSource.Telegram => new DeleteFileTelegramCommand(request.Id),
            _ => throw new NotSupportedException($"Storage source {request.StorageSource} is not supported")
        };
    }

    public static ICommand<AppResponse<DownloadFileResponse>> GetDownloadFileCommand(DownloadFileRequest request)
    {
        return request.StorageSource switch
        {
            StorageSource.R2 => new DownloadFileCommand(request.Id, request.StorageSource),
            StorageSource.Telegram => new DownloadFileTelegramCommand(request.Id),
            _ => throw new NotSupportedException($"Storage source {request.StorageSource} is not supported")
        };
    }   

}
