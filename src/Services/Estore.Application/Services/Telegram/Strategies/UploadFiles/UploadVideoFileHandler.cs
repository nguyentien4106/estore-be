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
                return await UploadWithFilePathAsync(args);
            }

            return await UploadWithFileStreamAsync(args);
        }
        catch(Exception ex){
            throw new Exception("Failed to upload video", ex);
        }
    }

    private static async Task<InputMediaUploadedDocument> UploadWithFilePathAsync(UploadFileHandlerArgs args){
        var attributes = TelegramServiceHelper.GetDocumentAttributes(args.FilePath);
        var videoUploaded = await args.Client.UploadFileAsync(args.FilePath, (transmitted, total) => {
            args.ProgressCallback?.Invoke(transmitted, total);
        });
        var video = new InputMediaUploadedDocument {
            file = videoUploaded, 
            mime_type = FileHelper.GetMimeTypeTelegram(args.FileName),
            attributes = attributes,
        };  

        return video;
    }

    private static async Task<InputMediaUploadedDocument> UploadWithFileStreamAsync(UploadFileHandlerArgs args){
        using(args.FileStream){
            var videoUploaded = await args.Client.UploadFileAsync(args.FileStream, args.FileName, (transmitted, total) => {
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
}
