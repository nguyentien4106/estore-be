using Estore.Application.Helpers;
using TL;

namespace Estore.Application.Strategies.UploadFiles;

public class UploadVideoFileHandler : IUploadFileHandler
{
    public async Task<InputMedia> UploadFileAsync(UploadFileHandlerArgs args)
    {
        try{
            var videoUploaded = await args.Client.UploadFileAsync(args.FileStream, args.FileName);

            var video = new InputMediaUploadedDocument {
                file = videoUploaded, 
                mime_type = "video/" + FileHelper.GetFileExtension(args.FileName),
                attributes = new[] {
                    new DocumentAttributeVideo {
                        w = args.Width, 
                        h = args.Height,
                        flags = DocumentAttributeVideo.Flags.supports_streaming,
                        
                    },
                },
                
            };

            return video;
        }
        catch(Exception ex){
            throw new Exception("Failed to upload video", ex);
        }
    }
}
