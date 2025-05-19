using System.Text.Json;
using EStore.Application.Helpers;
using EStore.Application.Models.Files;
using EStore.Application.Services.RabbitMQ;
using EStore.Application.Services.Telegram;

namespace EStore.Application.Commands.Files.UploadFileMultipart;

public class UploadFileMultipartHandler(
    IRabbitMQService queueService,
    ITelegramService telegramService,
    UserManager<User> userManager,
    IEStoreDbContext context) : ICommandHandler<UploadFileMultipartCommand, AppResponse<ChunkMessage>>
{
    public async Task<AppResponse<ChunkMessage>> Handle(UploadFileMultipartCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user is null)
        {
            return AppResponse<ChunkMessage>.NotFound("User", request.UserName);
        }

        var filePath = await HandleChunkFile(request);
        var id = Guid.Empty;
        if (request.ChunkIndex == request.TotalChunks - 1)
        {
            var telegramFile = new TeleFileEntity
            {
                FileName = request.FileName,
                FileSize = request.File.Length,
                FileType = FileHelper.DetermineFileType(request.FileName),
                UserId = request.UserId,
                ContentType = request.File.ContentType,
                Extension = Path.GetExtension(request.FileName),
                FileStatus = FileStatus.Uploading,
            };
            await context.TeleFileEntities.AddAsync(telegramFile, cancellationToken);
            await context.CommitAsync(cancellationToken);
            id = telegramFile.Id;
        }

        var chunkMessage = await SendToQueueAsync(request, filePath, id);

        return AppResponse<ChunkMessage>.Success(chunkMessage);
    }

    private async Task<string> HandleChunkFile(UploadFileMultipartCommand command)
    {
        var file = command.File;
        var fileId = command.FileId;
        var chunkIndex = command.ChunkIndex;
        using var stream = file.OpenReadStream();

        // store to local disk
        var filePath = Path.Combine(AppContext.BaseDirectory, "temps", command.UserId, command.FileId, chunkIndex.ToString());

        // Ensure the directory exists before writing the file
        var directoryPath = Path.GetDirectoryName(filePath);
        if (directoryPath != null && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath); // This creates all directories in the path if they don't exist
        }

        // Write chunk to file
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
        await queueService.ProducerAsync(message);

        return chunkMessage;
    }
}
