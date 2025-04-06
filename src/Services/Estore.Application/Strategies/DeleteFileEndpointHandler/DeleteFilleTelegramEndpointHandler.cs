using EStore.Application.Services.Telegram;
using TL;

namespace EStore.Application.Strategies.DeleteFileEndpointHandler;

public class DeleteFileTelegramEndpointHandler() : IDeleteFileEndpointHandler
{
    public async Task<AppResponse<Guid>> DeleteFileAsync(Guid id, CancellationToken cancellationToken){
        return AppResponse<Guid>.Success(id);
    }

}
