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
    private Client _client;

    private ChatBase? _peer;
    private readonly TelegramConfiguration _telegramConfiguration;

    public TelegramService(TelegramConfiguration telegramConfiguration)
    {
        _telegramConfiguration = telegramConfiguration;
        var result = CreateAndConnect().GetAwaiter().GetResult();
        if(result){
            _peer = _client?.Messages_GetAllChats().Result.chats[_telegramConfiguration.ChannelId];
        }
    }

    public async Task<AppResponse<bool>> DeleteMessageAsync(int messageId)
    {
        try{
            var reult = await _client.DeleteMessages(_peer, messageId);
            return AppResponse<bool>.Success(true);
        }
        catch(Exception ex){
            return AppResponse<bool>.Error(ex.Message);
        }
    }

    public async Task<AppResponse<Stream>> DownloadFileAsync(TeleFileEntity fileLocation)
    {
        try{
            Stream fileStream = new MemoryStream();
            var downloadFileHandler = TelegramFileHandlerFactory.GetDownloadFileHandler(fileLocation.FileType);
            var location = downloadFileHandler.GetLocation(fileLocation);
            var result = await _client.DownloadFileAsync(location, fileStream);
            fileStream.Position = 0;
            
            return AppResponse<Stream>.Success(fileStream);
        }
        catch(Exception ex){
            return AppResponse<Stream>.Error(ex.Message);
        }
    }

    public async Task<AppResponse<TeleFileEntity>> UploadFileToStrorageAsync(UploadFileTelegramCommand command, string userId)
    {
        var file = command.File;
        var fileStream = FileHelper.GetFileStream(file);
        var fileHandler = TelegramFileHandlerFactory.GetUploadFileHandler(FileHelper.DetermineFileType(file.FileName));

        var fileUploaded = await fileHandler.UploadFileAsync(new UploadFileHandlerArgs{
            Client = _client,
            FileStream = fileStream,
            Caption = file.FileName,
            FileName = file.FileName,
            Width = command.Width,
            Height = command.Height,
            ContentType = command.File.ContentType
        });
       
        var result = await _client.SendMessageAsync(_peer, userId, fileUploaded);
        var messageId = result.id;

        if (result.media != null)
        {
            return CreateTeleFileLocationFromMedia(result.media, file, command.Width, command.Height, userId, messageId);
        }

        return AppResponse<TeleFileEntity>.Error("Failed to upload file");
    }

    private AppResponse<TeleFileEntity> CreateTeleFileLocationFromMedia(MessageMedia media, IFormFile file, int width, int height, string userId, int messageId)
    {
        var teleFile = CreateBaseTeleFileEntity(file, userId, messageId);

        if (media is MessageMediaPhoto mediaPhoto && mediaPhoto.photo is Photo photo)
        {
            return HandlePhotoMedia(teleFile, photo, width, height);
        }

        if (media is MessageMediaDocument mediaDocument && mediaDocument.document is Document document)
        {
            return HandleDocumentMedia(teleFile, document, width, height);
        }

        return AppResponse<TeleFileEntity>.Error("Unsupported media type");
    }

    private static TeleFileEntity CreateBaseTeleFileEntity(IFormFile file, string userId, int messageId)
    {
        return new TeleFileEntity()
        {
            FileName = file.FileName,
            FileSize = file.Length,
            FileType = FileHelper.DetermineFileType(file.FileName),
            Extension = Path.GetExtension(file.FileName).TrimStart('.'),
            UserId = userId,
            MessageId = messageId,
            ContentType = file.ContentType
        };
    }

    private static AppResponse<TeleFileEntity> HandlePhotoMedia(TeleFileEntity teleFile, Photo photo, int width, int height)
    {
        var photoFileLocation = photo.ToFileLocation();
        var photoSize = photo.sizes.LastOrDefault();
        
        teleFile.FileId = photoFileLocation.id;
        teleFile.AccessHash = photoFileLocation.access_hash;
        teleFile.Flags = (uint)photo.flags;
        teleFile.FileReference = photoFileLocation.file_reference;
        teleFile.DcId = photo.dc_id;
        teleFile.Width = photoSize?.Width ?? width;
        teleFile.Height = photoSize?.Height ?? height;
        teleFile.Thumbnail = photoSize?.Type ?? "";

        return AppResponse<TeleFileEntity>.Success(teleFile);
    }

    private static AppResponse<TeleFileEntity> HandleDocumentMedia(TeleFileEntity teleFile, Document document, int width, int height)
    {
        var documentFileLocation = document.ToFileLocation();
        var documentSize = document.LargestThumbSize;
        
        teleFile.FileId = documentFileLocation.id;
        teleFile.AccessHash = documentFileLocation.access_hash;
        teleFile.Flags = (uint)document.flags;
        teleFile.FileReference = documentFileLocation.file_reference;
        teleFile.DcId = document.dc_id;
        teleFile.Width = documentSize?.Width ?? width;
        teleFile.Height = documentSize?.Height ?? height;
        teleFile.Thumbnail = documentSize?.Type ?? "v";

        return AppResponse<TeleFileEntity>.Success(teleFile);
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

    private async Task<bool> CreateAndConnect()
    {
        _client = new Client(Config);
        _client.OnUpdates += Client_OnUpdates;
        _client.OnOther += Client_OnOther;
        var my = await _client.LoginUserIfNeeded();
        Console.WriteLine($"We are logged-in as " + my);

        return true;
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
