namespace EStore.Application.Strategies.DeleteFileEndpointHandler;

public interface IDeleteFileEndpointHandler
{
    Task<AppResponse<Guid>> DeleteFileAsync(Guid id, CancellationToken cancellationToken);
}