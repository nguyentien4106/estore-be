using EStore.Domain.Models.Base;
using Microsoft.AspNetCore.Http;

namespace EStore.Application.Services.Cloudflare;

public interface ICloudflareClient
{
    Task<AppResponse<FileEntity>> UploadFileAsync(IFormFile file, string userName);

    Task<AppResponse<string>> DeleteFileAsync(string fileName);

    Task<AppResponse<string>> GeneratePresignedUrl(string fileKey);

    Task<AppResponse<Stream>> DownloadFile(string fileKey);

}
