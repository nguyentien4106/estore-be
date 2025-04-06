using Estore.Domain.Models.Base;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Services.Cloudflare;

public interface ICloudflareClient
{
    Task<AppResponse<FileEntity>> UploadFileAsync(IFormFile file, string userName);

    Task<AppResponse<string>> DeleteFileAsync(string fileName);

    Task<AppResponse<R2File>> GetFileByNameAsync(string fileName);

    Task<AppResponse<List<R2File>>> GetFilesByUserNameAsync(string userName);

    Task<AppResponse<string>> GeneratePresignedUrl(string fileKey);

}
