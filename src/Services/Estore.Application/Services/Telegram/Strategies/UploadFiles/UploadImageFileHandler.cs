using EStore.Application.Models.Files;
using TL;

namespace EStore.Application.Services.Telegram.Strategies.UploadFiles;

public class UploadImageFileHandler() : IUploadFileHandler
{
    public async Task<InputMedia> UploadFileAsync(UploadFileHandlerArgs args)
    {
        try
        {
            var photoUploaded = await args.Client.UploadFileAsync(args.FileStream, args.FileName, (long transmitted, long total) => {
                args.ProgressCallback?.Invoke(transmitted, total);
            });

            var image = new InputMediaUploadedPhoto {
                file = photoUploaded
            };

            return image;
        }
        catch (Exception ex) 
        {
            throw new Exception("Failed to upload image", ex);
        }
    }
}
