using EStore.Application.Helpers;
using EStore.Application.Models.Files;
using TL;

namespace EStore.Application.Services.Telegram.Strategies.UploadFiles;

public class UploadVideoFileHandler : IUploadFileHandler
{
    public async Task<InputMedia> UploadFileAsync(UploadFileHandlerArgs args)
    {
        try{
            if(!string.IsNullOrEmpty(args.FilePath)){
                var attributes = TelegramServiceHelper.GetDocumentAttributes(args.FilePath);
                var videoUploaded = await args.Client.UploadFileAsync(args.FilePath, (long transmitted, long total) => {
                    args.ProgressCallback?.Invoke(transmitted, total);
                });
                var video = new InputMediaUploadedDocument {
                    file = videoUploaded, 
                    mime_type = FileHelper.GetMimeTypeTelegram(args.FileName),
                    attributes = attributes,
                };  

                return video;
            }

            using(args.FileStream){
                var videoUploaded = await args.Client.UploadFileAsync(args.FileStream, args.FileName, (long transmitted, long total) => {
                    args.ProgressCallback?.Invoke(transmitted, total);
                });
                var video = new InputMediaUploadedDocument {
                    file = videoUploaded, 
                    mime_type = FileHelper.GetMimeTypeTelegram(args.FileName),
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
