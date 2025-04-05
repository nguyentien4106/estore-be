using Estore.Application.Strategies.DeleteFileEndpointHandler;
using Estore.Application.Strategies.UploadFileEndpointHandler;

namespace Estore.Application.Strategies;

public class EndpointHandlerFactory
{
    public static IUploadFileEndpointHandler GetUploadFileEndpointHandler(StorageSource storageSource)
    {
        return storageSource switch
        {
            StorageSource.R2 => new UploadFileR2EndpointHandler(),
            StorageSource.Telegram => new UploadFileTelegramEndpointHandler(),
            _ => throw new NotSupportedException($"Storage source {storageSource} is not supported")
        };
    }

    public static IDeleteFileEndpointHandler GetDeleteFileEndpointHandler(StorageSource storageSource)
    {
        return storageSource switch
        {
            StorageSource.R2 => new DeleteFileR2EndpointHandler(),
            StorageSource.Telegram => new DeleteFileTelegramEndpointHandler(),
            _ => throw new NotSupportedException($"Storage source {storageSource} is not supported")
        };
    }
}
