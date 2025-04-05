using Estore.Application.Services.Telegram;
using TL;

namespace Estore.Application.Strategies.DeleteFileEndpointHandler;

public class DeleteFileTelegramEndpointHandler() : IDeleteFileEndpointHandler
{
    public async Task<AppResponse<Guid>> DeleteFileAsync(Guid id, CancellationToken cancellationToken){
        return AppResponse<Guid>.Success(id);
    }

}
