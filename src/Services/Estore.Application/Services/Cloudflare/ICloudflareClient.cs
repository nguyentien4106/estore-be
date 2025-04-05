namespace Estore.Application.Services.Cloudflare;

public interface ICloudflareClient
{
    Task<AppResponse<R2File>> UploadFileAsync(string fileName, Stream fileStream, string contentType);

    Task<AppResponse<R2File>> DeleteFileAsync(string fileName);

    Task<AppResponse<R2File>> GetFileByNameAsync(string fileName);

    Task<AppResponse<List<R2File>>> GetFilesByUserNameAsync(string userName);

}
