namespace EStore.Application.Strategies.UploadFileEndpointHandler;

public interface IUploadFileEndpointHandler
{
    Task<AppResponse<Guid>> UploadFileAsync(Guid id, CancellationToken cancellationToken);
}