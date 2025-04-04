using TL;

namespace Estore.Application.Strategies.UploadFiles;

public class UploadImageFileHandler() : IUploadFileHandler
{
    public async Task<InputMedia> UploadFileAsync(UploadFileHandlerArgs args)
    {
        try
        {
            var photoUploaded = await args.Client.UploadFileAsync(args.FileStream, args.FileName);

            var image = new InputMediaUploadedPhoto {
                file = photoUploaded,
            };

            return image;
        }
        catch (Exception ex) 
        {
            throw new Exception("Failed to upload image", ex);
        }
    }
}
