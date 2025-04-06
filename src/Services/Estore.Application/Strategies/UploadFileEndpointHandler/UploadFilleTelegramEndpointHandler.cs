using EStore.Application.Services.Telegram;
using TL;

namespace EStore.Application.Strategies.UploadFileEndpointHandler;

public class UploadFileTelegramEndpointHandler() : IUploadFileEndpointHandler
{
    public async Task<AppResponse<Guid>> UploadFileAsync(Guid id, CancellationToken cancellationToken){
        return AppResponse<Guid>.Success(id);
    }

}
