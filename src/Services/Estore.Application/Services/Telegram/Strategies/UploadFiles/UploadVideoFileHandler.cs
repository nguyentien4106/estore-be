using EStore.Application.Helpers;
using EStore.Application.Models.Files;
using TL;

namespace EStore.Application.Services.Telegram.Strategies.UploadFiles;

public class UploadVideoFileHandler : IUploadFileHandler
{
    public async Task<InputMedia> UploadFileAsync(UploadFileHandlerArgs args)
    {
        try{
            Console.WriteLine($"Upload File Path {args.FilePath}");

            if(!string.IsNullOrEmpty(args.FilePath)){
                var attributes = TelegramServiceHelper.GetDocumentAttributes(args.FilePath);
                var videoUploaded = await args.Client.UploadFileAsync(args.FilePath);
                var video = new InputMediaUploadedDocument {
                    file = videoUploaded, 
                    mime_type = FileHelper.GetMimeTypeTelegram(args.FileName),//args.ContentType,
                    attributes = attributes,
                };

                return video;
            }
            Console.WriteLine($"Upload File Stream");

            using(args.FileStream){
                var videoUploaded = await args.Client.UploadFileAsync(args.FileStream, args.FileName);
                var video = new InputMediaUploadedDocument {
                    file = videoUploaded, 
                    mime_type = FileHelper.GetMimeTypeTelegram(args.FileName),//args.ContentType,
                    attributes = [
                        new DocumentAttributeVideo {
                            flags = DocumentAttributeVideo.Flags.supports_streaming,
                        },
                    ],
                    
                };

                return video;
            }
        }
        catch(Exception ex){
            throw new Exception("Failed to upload video", ex);
        }
    }
}
