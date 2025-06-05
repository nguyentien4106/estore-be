using EStore.Application.Models.Files;
using TL;

namespace EStore.Application.Services.Telegram.Strategies.UploadFiles;

public class UploadDocumentFileHandler : IUploadFileHandler
{
    public async Task<InputMedia> UploadFileAsync(UploadFileHandlerArgs args)
    {
        try{
            var documentUploaded = await args.Client.UploadFileAsync(args.FileStream, args.FileName, (long transmitted, long total) => {
                args.ProgressCallback?.Invoke(transmitted, total);
            });

            var document = new InputMediaUploadedDocument {
                file = documentUploaded, 
                mime_type = args.ContentType,
            };

            return document;
        }
        catch(Exception ex){
            throw new Exception("Failed to upload video", ex);
        }
    }
}
