using Estore.Application.Services.Telegram;
using TL;

namespace Estore.Application.Strategies.UploadFileEndpointHandler;

public class UploadFileTelegramEndpointHandler() : IUploadFileEndpointHandler
{
    public async Task<AppResponse<Guid>> UploadFileAsync(Guid id, CancellationToken cancellationToken){
        return AppResponse<Guid>.Success(id);
    }

}
