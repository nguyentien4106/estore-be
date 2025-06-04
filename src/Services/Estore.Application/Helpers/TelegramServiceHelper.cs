using TL;
using MediaInfo.DotNetWrapper.Enumerations;
using MediaInfo.DotNetWrapper;
using EStore.Application.Models.Files;
namespace EStore.Application.Helpers;

public static class TelegramServiceHelper
{

    public static DocumentAttribute[] GetDocumentAttributes(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var mediaInfo = new MediaInfo.DotNetWrapper.MediaInfo();
        mediaInfo.Open(filePath);
        var videoInfo =  new VideoInfo(mediaInfo);
        string fileName = Path.GetFileName(filePath);

        // If no video stream found (width/height/duration == 0), treat it as image or document
        if (videoInfo.Width == 0 || videoInfo.Heigth == 0 || string.IsNullOrWhiteSpace(videoInfo.Codec))
        {
            return new DocumentAttribute[]
            {
                new DocumentAttributeFilename { file_name = fileName }
            };
        }

        return new DocumentAttribute[]
        {
            new DocumentAttributeVideo
            {
                w = videoInfo.Width,
                h = videoInfo.Heigth,
                duration = (int)videoInfo.Duration.TotalSeconds,
                flags = DocumentAttributeVideo.Flags.supports_streaming
            },
            new DocumentAttributeFilename
            {
                file_name = fileName
            }
        };
    }

    public static string GetMimeType(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext switch
        {
            ".mp4" => "video/mp4",
            ".mov" => "video/quicktime",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webm" => "video/webm",
            ".mkv" => "video/x-matroska",
            _ => "application/octet-stream"
        };
    }
    
    public static AppResponse<TeleFileEntity> CreateTeleFileLocationFromMedia(MessageMedia media, UploadFileHandlerArgs args, string userId, int messageId)
    {
        var teleFile = new TeleFileEntity
        {
            FileName = args.FileName,
            FileSize = args.ContentLength,
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

    public static AppResponse<TeleFileEntity> HandlePhotoMedia(TeleFileEntity teleFile, Photo photo)
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

    public static AppResponse<TeleFileEntity> HandleDocumentMedia(TeleFileEntity teleFile, Document document)
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

    public static string GetVerificationCode()
    {
        Console.Write("Code: ");
        return Console.ReadLine() ?? string.Empty;
    }

    public static Task HandleUpdates(UpdatesBase updates)
    {
        foreach (var update in updates.UpdateList)
        {
            Console.WriteLine(update.GetType().Name);
            switch (update)
            {
                case UpdateChannel updateChannel:
                    Console.WriteLine(updateChannel.channel_id);
                    Console.WriteLine(updateChannel.GetMBox());
                    break;
            }
        }
        
        return Task.CompletedTask;
    }
}
