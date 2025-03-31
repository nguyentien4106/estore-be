using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace Estore.Application.Services;

public interface ICloudflareClient
{
    Task<AppResponse<R2File>> UploadImageAsync(string fileName, Stream fileStream, string contentType);

    Task<AppResponse<R2File>> DeleteImageAsync(string fileName);

    Task<AppResponse<R2File>> GetImageUrlAsync(string fileName);

    Task<AppResponse<List<R2File>>> GetImagesByUserNameAsync(string userName);

}
