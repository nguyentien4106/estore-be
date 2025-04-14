using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using EStore.Domain.Models.Base;
using EStore.Application.Helpers;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace EStore.Application.Services.Cloudflare;

public class CloudflareClient : ICloudflareClient
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private CloudflareConfiguration _cloudflareConfiguration;
    private HttpStatusCode[] SuccessCodes = [HttpStatusCode.OK, HttpStatusCode.NoContent, HttpStatusCode.Accepted, HttpStatusCode.Created];
   
    public CloudflareClient(CloudflareConfiguration cloudflareConfiguration)
    {
        _cloudflareConfiguration = cloudflareConfiguration;
        var config = new AmazonS3Config
        {
            ServiceURL = $"https://{cloudflareConfiguration.AccountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true
        };

        var credentials = new BasicAWSCredentials(cloudflareConfiguration.AccessKey, cloudflareConfiguration.SecretKey);

        _s3Client = new AmazonS3Client(credentials, config);
        _bucketName = cloudflareConfiguration.BucketName;
    }

    public async Task<AppResponse<FileEntity>> UploadFileAsync(IFormFile file, string userName)
    {
        var fileName = file.FileName;
        var contentType = file.ContentType;
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = R2Helper.GetR2FileKey(userName, fileName),
            InputStream = FileHelper.GetMemoryStream(file),
            ContentType = file.ContentType,
            DisablePayloadSigning = true,
        };

        var response = await _s3Client.PutObjectAsync(putRequest);

        if (SuccessCodes.Contains(response.HttpStatusCode)) 
        {
            return AppResponse<FileEntity>.Success(new ()
            {
                FileName = fileName,
                FileSize = file.Length,
                FileType = FileHelper.DetermineFileType(fileName),
                Extension = FileHelper.GetFileExtension(fileName),
                ContentType = contentType,
            });
        }

        return AppResponse<FileEntity>.Error(response.HttpStatusCode.ToString());
    }

    // 📌 Delete Image from Cloudflare R2
    public async Task<AppResponse<string>> DeleteFileAsync(string fileName)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        var response = await _s3Client.DeleteObjectAsync(deleteRequest);
        if (SuccessCodes.Contains(response.HttpStatusCode))
        {
            return AppResponse<string>.Success(fileName);
        }
        
        return AppResponse<string>.Error(response.HttpStatusCode.ToString());
    }

    public async Task<AppResponse<string>> GeneratePresignedUrl(string fileKey)
    {
        try{
            AWSConfigsS3.UseSignatureVersion4 = true;
            var presign = new GetPreSignedUrlRequest
            {
                BucketName = _cloudflareConfiguration.BucketName,
                Key = fileKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddDays(7),
            };
            var url = await _s3Client.GetPreSignedURLAsync(presign);
                
            return AppResponse<string>.Success(url);
        }
        catch(Exception ex){
            return AppResponse<string>.Error(ex.Message);
        }
    }

    public async Task<AppResponse<Stream>> DownloadFile(string fileKey)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _cloudflareConfiguration.BucketName,
                Key = fileKey
            };

            using var response = await _s3Client.GetObjectAsync(request);
            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            
            return AppResponse<Stream>.Success(memoryStream);
        }
        catch(Exception ex)
        {
            return AppResponse<Stream>.Error(ex.Message);
        }
    }
}
