using System.Text.Json;
using EStore.Application.Constants;
using EStore.Application.Extensions;
using EStore.Application.Helpers;
using EStore.Application.Models.Files;
using EStore.Application.Services.RabbitMQ;

namespace EStore.Application.Commands.Files.UploadFileMultipart;

public class UploadFileMultipartHandler(
    IRabbitMQService queueService,
    UserManager<User> userManager,
    IEStoreDbContext context) : ICommandHandler<UploadFileMultipartCommand, AppResponse<FileEntityResult>>
{
    public async Task<AppResponse<FileEntityResult>> Handle(UploadFileMultipartCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user is null)
        {
            return AppResponse<FileEntityResult>.NotFound("User", request.UserName);
        }

        var filePath = await HandleChunkFile(request);
        var id = Guid.Empty;
        TeleFileEntity telegramFile = null;

        if (request.ChunkIndex == request.TotalChunks - 1)
        {
            telegramFile = new TeleFileEntity
            {
                FileName = request.FileName,
                FileSize = request.File.Length,
                FileType = FileHelper.DetermineFileType(request.FileName),
                UserId = request.UserId,
                ContentType = request.ContentType,
                Extension = Path.GetExtension(request.FileName),
                FileStatus = FileStatus.Uploading,
            };
            await context.TeleFileEntities.AddAsync(telegramFile, cancellationToken);
            await context.CommitAsync(cancellationToken);
            id = telegramFile.Id;
        }

        await SendToQueueAsync(request, filePath, id);

        return AppResponse<FileEntityResult>.Success(telegramFile?.ToFileEntityResponse());
    }

    private async Task<string> HandleChunkFile(UploadFileMultipartCommand command)
    {
        var file = command.File;
        var chunkIndex = command.ChunkIndex;
        using var stream = file.OpenReadStream();

        var filePath = FileHelper.GetTempFilePathPart(command.UserId, command.FileId, chunkIndex);

        var directoryPath = Path.GetDirectoryName(filePath);
        if (directoryPath != null && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using (FileStream outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await stream.CopyToAsync(outputStream);
        }

        return filePath;
    }

    private async Task<ChunkMessage> SendToQueueAsync(UploadFileMultipartCommand request, string filePath, Guid id)
    {
        var chunkMessage = new ChunkMessage
        {
            FileId = request.FileId,
            UserId = request.UserId,
            FilePath = filePath,
            ChunkIndex = request.ChunkIndex,
            TotalChunks = request.TotalChunks,
            FileName = request.FileName,
            Id = id
        };

        var message = JsonSerializer.Serialize(chunkMessage);
        await queueService.ProducerAsync(QueueConstants.MergeFileQueue, message);

        return chunkMessage;
    }
}
