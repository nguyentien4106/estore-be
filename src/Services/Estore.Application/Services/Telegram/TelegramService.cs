using Microsoft.Extensions.Logging;
using TdLib;
using Microsoft.AspNetCore.Http;
using EStore.Application.Helpers;
using EStore.Application.Services.Files;
using WTelegram;
using TL;
using EStore.Application.Files.Commands.UploadFileTelegram;

namespace EStore.Application.Services.Telegram;

public class TelegramService : ITelegramService
{
    private Client? _client;
    private ChatBase? _peer;
    private readonly TelegramConfiguration _telegramConfiguration;
    private readonly ILogger<TelegramService> _logger;

    public TelegramService(TelegramConfiguration telegramConfiguration, ILogger<TelegramService> logger)
    {
        _telegramConfiguration = telegramConfiguration;
        _logger = logger;
        
        if (!InitializeClient().GetAwaiter().GetResult())
        {
            throw new InvalidOperationException("Failed to connect to Telegram");
        }
    }

    private async Task<bool> InitializeClient()
    {
        try
        {
            _client = new Client(Config);
            _client.OnUpdates += HandleUpdates;
            _client.OnOther += HandleOtherEvents;
            
            var user = await _client.LoginUserIfNeeded();
            _logger.LogInformation("Logged in as {User}", user);
            
            var chats = await _client.Messages_GetAllChats();
            _peer = chats.chats[_telegramConfiguration.ChannelId];
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Telegram client");
            return false;
        }
    }

    public async Task<AppResponse<bool>> DeleteMessageAsync(int messageId)
    {
        try
        {
            await _client.DeleteMessages(_peer, messageId);
            return AppResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete message {MessageId}", messageId);
            return AppResponse<bool>.Error(ex.Message);
        }
    }

    public async Task<AppResponse<Stream>> DownloadFileAsync(TeleFileEntity fileLocation)
    {
        try
        {
            var fileStream = new MemoryStream();
            var downloadHandler = TelegramFileHandlerFactory.GetDownloadFileHandler(fileLocation.FileType);
            var location = downloadHandler.GetLocation(fileLocation);
            
            await _client.DownloadFileAsync(location, fileStream);
            fileStream.Position = 0;
            
            return AppResponse<Stream>.Success(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {FileId}", fileLocation.FileId);
            return AppResponse<Stream>.Error(ex.Message);
        }
    }

    public async Task<AppResponse<TeleFileEntity>> UploadFileToStrorageAsync(UploadFileTelegramCommand command, string userId)
    {
        var file = command.File;
        using var fileStream = FileHelper.GetFileStream(file);
        
        var args = new UploadFileHandlerArgs
        {
            Client = _client,
            FileStream = fileStream,
            Caption = file.FileName,
            FileName = file.FileName,
            ContentType = file.ContentType
        };

        return await UploadFileToStrorageAsync(args, userId);
    }

    public async Task<AppResponse<TeleFileEntity>> UploadFileToStrorageAsync(UploadFileHandlerArgs args, string userId)
    {
        try
        {
            var fileType = FileHelper.DetermineFileType(args.FileName);
            var uploadHandler = TelegramFileHandlerFactory.GetUploadFileHandler(fileType);
            var uploadedFile = await uploadHandler.UploadFileAsync(args);

            var message = await _client.SendMessageAsync(_peer, userId, uploadedFile);
            
            return message.media != null 
                ? CreateTeleFileLocationFromMedia(message.media, args, userId, message.id)
                : AppResponse<TeleFileEntity>.Error("Failed to upload file");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName}", args.FileName);
            return AppResponse<TeleFileEntity>.Error(ex.Message);
        }
    }

    private static AppResponse<TeleFileEntity> CreateTeleFileLocationFromMedia(MessageMedia media, UploadFileHandlerArgs args, string userId, int messageId)
    {
        var teleFile = new TeleFileEntity
        {
            FileName = args.FileName,
            FileSize = args.FileStream.Length,
            FileType = FileHelper.DetermineFileType(args.FileName),
            Extension = Path.GetExtension(args.FileName).TrimStart('.'),
            UserId = userId,
            MessageId = messageId,
            ContentType = args.ContentType
        };

        return media switch
        {
            MessageMediaPhoto { photo: Photo photo } => HandlePhotoMedia(teleFile, photo),
            MessageMediaDocument { document: Document document } => HandleDocumentMedia(teleFile, document),
            _ => AppResponse<TeleFileEntity>.Error("Unsupported media type")
        };
    }

    private static AppResponse<TeleFileEntity> HandlePhotoMedia(TeleFileEntity teleFile, Photo photo)
    {
        var location = photo.ToFileLocation();
        var size = photo.sizes.LastOrDefault();

        teleFile.FileId = location.id;
        teleFile.AccessHash = location.access_hash;
        teleFile.Flags = (uint)photo.flags;
        teleFile.FileReference = location.file_reference;
        teleFile.DcId = photo.dc_id;
        teleFile.Thumbnail = size?.Type ?? "w";

        return AppResponse<TeleFileEntity>.Success(teleFile);
    }

    private static AppResponse<TeleFileEntity> HandleDocumentMedia(TeleFileEntity teleFile, Document document)
    {
        var location = document.ToFileLocation();

        teleFile.FileId = location.id;
        teleFile.AccessHash = location.access_hash;
        teleFile.Flags = (uint)document.flags;
        teleFile.FileReference = location.file_reference;
        teleFile.DcId = document.dc_id;
        teleFile.Thumbnail = "v";

        return AppResponse<TeleFileEntity>.Success(teleFile);
    }

    private string Config(string what) => what switch
    {
        "api_id" => _telegramConfiguration.ApiId.ToString(),
        "api_hash" => _telegramConfiguration.ApiHash,
        "phone_number" => _telegramConfiguration.PhoneNumber,
        "verification_code" => GetVerificationCode(),
        "first_name" => "EStore",
        "last_name" => "Bot", 
        "password" => _telegramConfiguration.TwoFactorPassword,
        _ => null
    };

    private static string GetVerificationCode()
    {
        Console.Write("Code: ");
        return Console.ReadLine() ?? string.Empty;
    }

    private async Task HandleOtherEvents(IObject arg)
    {
        if (arg is not ReactorError err) 
        {
            _logger.LogInformation("Other event: {Type}", arg.GetType().Name);
            return;
        }

        _logger.LogError(err.Exception, "Fatal reactor error");
        await ReconnectClient();
    }

    private async Task ReconnectClient()
    {
        while (true)
        {
            _logger.LogInformation("Attempting to reconnect in 5 seconds...");
            if (_client != null) await _client.DisposeAsync();
            
            await Task.Delay(5000);
            
            try
            {
                await InitializeClient();
                break;
            }
            catch (Exception ex) when (ex is not ObjectDisposedException)
            {
                _logger.LogError(ex, "Reconnection failed");
            }
        }
    }

    private static Task HandleUpdates(UpdatesBase updates)
    {
        foreach (var update in updates.UpdateList)
        {
            Console.WriteLine(update.GetType().Name);
        }
        return Task.CompletedTask;
    }
}
