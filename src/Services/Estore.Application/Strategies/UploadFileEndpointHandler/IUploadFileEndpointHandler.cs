namespace Estore.Application.Strategies.UploadFileEndpointHandler;

public interface IUploadFileEndpointHandler
{
    Task<AppResponse<Guid>> UploadFileAsync(Guid id, CancellationToken cancellationToken);
}