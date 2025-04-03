using Estore.Application.Helpers;
using TdLib;

namespace Estore.Application.Services.Files;

public class ImageFileHandler(TelegramConfiguration config) : IFileHandler
{
    public async Task<AppResponse<string>> UploadFileAsync(FileHandlerArgs args)
    {
        try
        {
            var result = await args.TdClient.ExecuteAsync(new TdApi.SendMessage()
            {
                ChatId = config.ChannelId,
                InputMessageContent = new TdApi.InputMessageContent.InputMessagePhoto
                {
                    Photo = new TdApi.InputFile.InputFileLocal { Path = args.LocalPath },
                    Caption = new TdApi.FormattedText { Text = args.Caption ?? string.Empty }
                }
            });
            DebugHelper.Log("SendMessage Result: ", result);
            
            return AppResponse<string>.Success(args.LocalPath);
        }
        catch (Exception ex) 
        {
            return AppResponse<string>.Error("Failed to upload image");

        }
    }
}
