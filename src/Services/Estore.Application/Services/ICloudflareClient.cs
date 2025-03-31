namespace Estore.Application.Services;

public interface ICloudflareClient
{
    Task<AppResponse<string>> UploadImageAsync(string fileName, Stream fileStream, string contentType);

    Task<bool> DeleteImageAsync(string fileName);

    Task<string> GetImageUrlAsync(string fileName);

    Task<List<string>> GetUserImagesAsync(string userName);
}
