using EStore.Application.Strategies.UploadFileEndpointHandler;
using TL;

namespace EStore.Application.Strategies.DeleteFileEndpointHandler;

public class UploadFileR2EndpointHandler : IUploadFileEndpointHandler
{
    public Task<AppResponse<Guid>> UploadFileAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
