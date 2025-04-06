using TL;

namespace EStore.Application.Strategies.DeleteFileEndpointHandler;

public class DeleteFileR2EndpointHandler : IDeleteFileEndpointHandler
{
    public Task<AppResponse<Guid>> DeleteFileAsync(Guid id){
        return Task.FromResult(AppResponse<Guid>.Success(id));
    }

    public Task<AppResponse<Guid>> DeleteFileAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
