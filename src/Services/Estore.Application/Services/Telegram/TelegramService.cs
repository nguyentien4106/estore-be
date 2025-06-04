using EStore.Application.Commands.Files.UploadFile;
using Microsoft.Extensions.Logging;
using EStore.Application.Helpers;
using EStore.Application.Services.Files;
using WTelegram;
using TL;
using EStore.Application.Models.Files;
using TL.Methods;
using System.Text.Json;
using EStore.Application.Data;
using Microsoft.Extensions.DependencyInjection;
namespace EStore.Application.Services.Telegram;

public class TelegramService : ITelegramService
{
    private Dictionary<long, ChatBase> _chats = [];
    private Client? _client;
    private ChatBase? _peer;

    private readonly TelegramConfiguration _telegramConfiguration;
    private readonly ILogger<TelegramService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TelegramService(
        TelegramConfiguration telegramConfiguration, 
        IServiceScopeFactory serviceScopeFactory,
        ILogger<TelegramService> logger)
    {
        _telegramConfiguration = telegramConfiguration;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

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
            //_client.MTProxyUrl = "https://t.me/proxy?server=87.229.100.252&port=443&secret=eeRighJJvXrFGRMCIMJdCQ";
            _client.OnUpdates += HandleUpdates;
            _client.OnOther += HandleOtherEvents;
            
            var user = await _client.LoginUserIfNeeded();
            _logger.LogInformation("Logged in as {User}", user);
            
            var chats = await _client.Messages_GetAllChats();
            _chats = chats.chats;
            _peer = chats.chats[_telegramConfiguration.ChannelId];
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Telegram client");
            return false;
        }
    }

    public async Task<AppResponse<(long, long)>> CreateNewChannelAsync(string channelName, string? description = null, CancellationToken cancellationToken = default)
    {
        try{
            var result = await _client.Channels_CreateChannel(channelName, description ?? "StoreChannel", null, null, 0, false, false, false, false);
            var channel = result.Chats.Values.First() as Channel;
            if(channel == null)
            {
                return AppResponse<(long, long)>.Error("Failed to create channel");
            }

            _chats.Add(channel.ID, channel);
            
            return AppResponse<(long, long)>.Success((channel.ID, channel.access_hash));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create channel {Channel}", channelName);
            return AppResponse<(long, long)>.Error(ex.Message);
        }

    }

    public async Task<AppResponse<bool>> DeleteChannelAsync(long channelId, long accessHash)
    {
        try
        {
            var inputChannel = new InputChannel(channelId, accessHash);
            await _client.Channels_DeleteChannel(inputChannel);
            _chats.Remove(channelId);

            return AppResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete channel {ChannelId}", channelId);
            return AppResponse<bool>.Error(ex.Message);
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

    public async Task<AppResponse<bool>> DeleteMessageAsync(List<int> messageIds)
    {
        try
        {
            await _client.DeleteMessages(_peer, [.. messageIds]);
            return AppResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete messages {MessageIds}", messageIds);
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
        using var fileStream = FileHelper.GetMemoryStream(file);
        var args = new UploadFileHandlerArgs
        {
            Client = _client,
            FileStream = fileStream,
            Caption = file.FileName,
            FileName = file.FileName,
            ContentType = file.ContentType,
            ContentLength = file.Length
        };

        return await UploadFileToStrorageAsync(args, userId);
        
    }

    public async Task<AppResponse<TeleFileEntity>> UploadFileToStrorageAsync(UploadFileHandlerArgs args, string userId)
    {
        try
        {
            var fileType = FileHelper.DetermineFileType(args.FileName);
            var uploadHandler = TelegramFileHandlerFactory.GetUploadFileHandler(fileType);
            args.Client = _client;
            var uploadedFile = await uploadHandler.UploadFileAsync(args);

            var message = await _client.SendMessageAsync(_peer, userId, uploadedFile);
            
            return message.media != null 
                ? TelegramServiceHelper.CreateTeleFileLocationFromMedia(message.media, args, userId, message.id)
                : AppResponse<TeleFileEntity>.Error("Failed to upload file");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName}", args.FileName);
            return AppResponse<TeleFileEntity>.Error(ex.Message);
        }
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
        /*foreach (var update in updates.UpdateList)
        {
            Console.WriteLine(update.GetType().Name);
            switch (update)
            {
                case UpdateChannel updateChannel:
                    Console.WriteLine(updateChannel.channel_id);
                    Console.WriteLine(updateChannel.GetMBox());
                    break;
            }
        }*/
        
        return Task.CompletedTask;
    }
}
