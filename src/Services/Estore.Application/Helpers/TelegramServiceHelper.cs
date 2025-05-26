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
    
    public static string GetCaption(string fileName){
        return $"File: {fileName}";
    }
}
