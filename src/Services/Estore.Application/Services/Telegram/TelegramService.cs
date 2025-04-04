using Microsoft.Extensions.Logging;
using TdLib;
using Microsoft.AspNetCore.Http;
using Estore.Application.Helpers;
using Estore.Application.Services.Files;
using WTelegram;
using TL;
using Estore.Application.Files.Commands.UploadFileTelegram;

namespace Estore.Application.Services.Telegram;

public class TelegramService : ITelegramService
{
    private Client _client;
    private readonly TelegramConfiguration _telegramConfiguration;

    public TelegramService(TelegramConfiguration telegramConfiguration)
    {
        _telegramConfiguration = telegramConfiguration;
        CreateAndConnect().GetAwaiter().GetResult();
    }

    public Task DeleteMessageAsync(long messageId)
    {
        throw new NotImplementedException();
    }

    public async Task<AppResponse<string>> DownloadFileAsync(TeleFileLocation fileLocation)
    {
        try{
            var filePath = FileHelper.CreateFilePathForDownload(fileLocation.FileName);
            using var fileStream = File.Create(filePath);
            var downloadFileHandler = FileHandlerFactory.GetDownloadFileHandler(fileLocation.FileType);
            var location = downloadFileHandler.GetLocation(fileLocation);
            var result = await _client.DownloadFileAsync(location, fileStream);

            return AppResponse<string>.Success(filePath);
        }
        catch(Exception ex){
            return AppResponse<string>.Error(ex.Message);
        }
    }

    public async Task<AppResponse<TeleFileLocation>> UploadFileToStrorageAsync(UploadFileTelegramCommand command, string userId)
    {
        var file = command.File;
        var chats = await _client.Messages_GetAllChats();
        var peer = chats.chats[_telegramConfiguration.ChannelId];
        var fileStream = FileHelper.GetFileStream(file);
        var fileHandler = FileHandlerFactory.GetUploadFileHandler(FileHelper.DetermineFileType(file.FileName));

        var fileUploaded = await fileHandler.UploadFileAsync(new UploadFileHandlerArgs{
            Client = _client,
            FileStream = fileStream,
            Caption = file.FileName,
            FileName = file.FileName,
            Width = command.Width,
            Height = command.Height
        });
       
        var result = await _client.SendMessageAsync(peer, userId, fileUploaded);

        if (result.media != null)
        {
            return CreateTeleFileLocationFromMedia(result.media, file, command.Width, command.Height, userId);
        }

        return AppResponse<TeleFileLocation>.Error("Failed to upload file");
    }

    private AppResponse<TeleFileLocation> CreateTeleFileLocationFromMedia(MessageMedia media, IFormFile file, int width, int height, string userId)
    {
        if (media is MessageMediaPhoto mediaPhoto && mediaPhoto.photo is Photo photo)
        {
            return CreateTeleFileLocationFromPhoto(photo, file, width, height, userId);
        }

        if (media is MessageMediaDocument mediaDocument && mediaDocument.document is Document document)
        {
            return CreateTeleFileLocationFromDocument(document, file, width, height, userId);
        }

        return AppResponse<TeleFileLocation>.Error("Unsupported media type");
    }

    private AppResponse<TeleFileLocation> CreateTeleFileLocationFromPhoto(Photo photo, IFormFile file, int width, int height, string userId)
    {
        var fileLocation = photo.ToFileLocation();
        var photoSize = photo.sizes.LastOrDefault();
        var location = TeleFileLocation.Create(
            fileLocation.id,
            fileLocation.access_hash,
            (uint)photo.flags,
            fileLocation.file_reference,
            photo.dc_id,
            photoSize?.Width ?? width,
            photoSize?.Height ?? height,
            file.FileName,
            file.Length,
            FileHelper.DetermineFileType(file.FileName),
            Path.GetExtension(file.FileName).TrimStart('.'),
            photoSize?.Type ?? "",
            userId
        );
        return AppResponse<TeleFileLocation>.Success(location);
    }

    private AppResponse<TeleFileLocation> CreateTeleFileLocationFromDocument(Document document, IFormFile file, int width, int height, string userId)
    {
        var fileLocation = document.ToFileLocation();
        var photoSize = document.LargestThumbSize;
        
        var location = TeleFileLocation.Create(
            fileLocation.id,
            fileLocation.access_hash,
            (uint)document.flags,
            fileLocation.file_reference,
            document.dc_id,
            photoSize?.Width ?? width,
            photoSize?.Height ?? height,
            file.FileName,
            file.Length,
            FileHelper.DetermineFileType(file.FileName),
            Path.GetExtension(file.FileName).TrimStart('.'),
            photoSize?.Type ?? "v",
            userId
        );
        return AppResponse<TeleFileLocation>.Success(location);
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
