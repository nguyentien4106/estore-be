using Microsoft.Extensions.Logging;
using TdLib;
using Microsoft.AspNetCore.Http;
using Estore.Application.Helpers;
using Estore.Application.Services.Files;
using WTelegram;
using TL;

namespace Estore.Application.Services.Telegram;

public class TelegramService : ITelegramService
{
    private Client _client;
    private readonly TelegramConfiguration _telegramConfiguration;
    private long access_hash = 1543940810101474296;

    public TelegramService(TelegramConfiguration telegramConfiguration, ILogger<TelegramService> logger)
    {
        _telegramConfiguration = telegramConfiguration;
        CreateAndConnect().GetAwaiter().GetResult();
    }

    public Task DeleteMessageAsync(long messageId)
    {
        throw new NotImplementedException();
    }

    public async Task<AppResponse<long>> DownloadFileAsync(TeleFileLocation fileLocation)
    {
        try{
             var photo = new Photo(){
                access_hash= fileLocation.AccessHash,
                id = fileLocation.FileId,
                flags = 0,
                sizes = [new PhotoSize(){
                    type = "y",
                    w = 1161,
                    h = 750,
                    size = 253931
                }],
                dc_id = 5,
                file_reference = fileLocation.FileReference
            };
            var location = photo.ToFileLocation();
            using var fileStream = File.Create(photo.id + ".png");

            var result = await _client.DownloadFileAsync(location, fileStream);
            return AppResponse<long>.Success(photo.id);
        }
        catch(Exception ex){
            return AppResponse<long>.Error(ex.Message);
        }
    }

    public async Task<AppResponse<TeleFileLocation>> UploadFileToStrorageAsync(IFormFile file, string userId)
    {
        var chats = await _client.Messages_GetAllChats();
        InputPeer peer = chats.chats[_telegramConfiguration.ChannelId];

        var inputFile = await _client.UploadFileAsync(FileHelper.GetFileStream(file), file.FileName);
        
        var photoInput =  new InputMediaUploadedPhoto { file = inputFile };

        var result = await _client.SendMessageAsync(peer, userId, photoInput);

        if(result.media is MessageMediaPhoto mediaPhoto && mediaPhoto.photo is Photo photo){
            var fileLocation = photo.ToFileLocation();
            var location = TeleFileLocation.Create(photo.id, photo.access_hash, fileLocation.thumb_size, fileLocation.file_reference);
            return AppResponse<TeleFileLocation>.Success(location);
        }
        
        return AppResponse<TeleFileLocation>.Error("Failed to upload file");
    }

    private string Config(string what)
    {
        switch (what)
        {
            case "api_id": return _telegramConfiguration.ApiId.ToString();
            case "api_hash": return _telegramConfiguration.ApiHash;
            case "phone_number": return _telegramConfiguration.PhoneNumber;
            case "verification_code": Console.Write("Code: "); return Console.ReadLine();
            case "first_name": return "EStore";      // if sign-up is required
            case "last_name": return "Bot";        // if sign-up is required
            case "password": return _telegramConfiguration.TwoFactorPassword;     // if user has enabled 2FA
            default: return null;                  // let WTelegramClient decide the default config
        }
    }

    private async Task CreateAndConnect()
    {
        _client = new Client(Config);
        _client.OnUpdates += Client_OnUpdates;
        _client.OnOther += Client_OnOther;
        var my = await _client.LoginUserIfNeeded();
        Console.WriteLine($"We are logged-in as " + my);
    }

    private async Task Client_OnOther(IObject arg)
    {
        if (arg is ReactorError err)
        {
            // typically: network connection was totally lost
            Console.WriteLine($"Fatal reactor error: {err.Exception.Message}");
            while (true)
            {
                Console.WriteLine("Disposing the client and trying to reconnect in 5 seconds...");
                if (_client != null) await _client.DisposeAsync();
                _client = null;
                await Task.Delay(5000);
                try
                {
                    await CreateAndConnect();
                    break;
                }
                catch (Exception ex) when (ex is not ObjectDisposedException)
                {
                    Console.WriteLine("Connection still failing: " + ex.Message);
                }
            }
        }
        else
            Console.WriteLine("Other: " + arg.GetType().Name);
    }

    private static Task Client_OnUpdates(UpdatesBase updates)
    {
        foreach (var update in updates.UpdateList)
            Console.WriteLine(update.GetType().Name);
        return Task.CompletedTask;
    }
}
