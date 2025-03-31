using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Estore.Application.Services;

public class CloudflareClient : ICloudflareClient
{
    private readonly AmazonS3Client _s3Client;
    private readonly string _bucketName;
    private readonly string _publicUrl; // Public R2 URL

    public CloudflareClient(CloudflareConfiguration cloudflareConfiguration)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = $"https://{cloudflareConfiguration.AccountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true, // Required for R2
            RegionEndpoint = RegionEndpoint.USEast2  // R2 doesn't use AWS regions, but this avoids SDK issues
        };

        _s3Client = new AmazonS3Client("ae79628138c83632aa161c985c36f0b3", "e2c99f0f4a06313757b6f83a585e63d18ead1714f1d7ca2d26d9a7a001389221", config);
        _bucketName = cloudflareConfiguration.BucketName;
        _publicUrl = $"https://{cloudflareConfiguration.BucketName}.r2.dev/"; // Cloudflare public URL
    }

    // 📌 Upload Image to Cloudflare R2
    public async Task<AppResponse<string>> UploadImageAsync(string fileName, Stream fileStream, string contentType)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = fileStream,
            ContentType = contentType
        };

        var response = await _s3Client.PutObjectAsync(putRequest);

        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) 
        {
            return AppResponse<string>.Success(GetFileName(fileName));
        }

        return AppResponse<string>.Error(nameof(response.HttpStatusCode));
    }

    // 📌 Delete Image from Cloudflare R2
    public async Task<bool> DeleteImageAsync(string fileName)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        var response = await _s3Client.DeleteObjectAsync(deleteRequest);
        return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
    }

    // 📌 Get Image Public URL
    public Task<string> GetImageUrlAsync(string fileName)
    {
        return Task.FromResult($"{_publicUrl}{fileName}");
    }

    public Task<List<string>> GetUserImagesAsync(string userName)
    {
        throw new NotImplementedException();
    }

    private string GetFileName(string fileName) => $"{_publicUrl}{fileName}";
}
