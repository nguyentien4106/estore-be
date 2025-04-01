using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace Estore.Application.Services;

public class CloudflareClient : ICloudflareClient
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private CloudflareConfiguration _cloudflareConfiguration;
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

    public async Task<AppResponse<R2File>> UploadFileAsync(string fileName, Stream fileStream, string contentType)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = fileStream,
            ContentType = contentType,
            DisablePayloadSigning = true,
        };

        var response = await _s3Client.PutObjectAsync(putRequest);

        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK) 
        {
            return AppResponse<R2File>.Success(new ()
            {
                Url = await GeneratePresignedUrl(fileName),
                FileName = fileName
            });
        }

        return AppResponse<R2File>.Error(nameof(response.HttpStatusCode));
    }

    // 📌 Delete Image from Cloudflare R2
    public async Task<AppResponse<R2File>> DeleteFileAsync(string fileName)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        var response = await _s3Client.DeleteObjectAsync(deleteRequest);
        if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return AppResponse<R2File>.Success(new() { FileName = fileName });
        }
        
        return AppResponse<R2File>.Error(response.HttpStatusCode.ToString());
    }

    // 📌 Get Image Public URL
    public async Task<AppResponse<R2File>> GetFileByNameAsync(string fileName)
    {
        var url = await GeneratePresignedUrl(fileName);
        return AppResponse<R2File>.Success(new () { Url = url, FileName = fileName });
    }

    public async Task<AppResponse<List<R2File>>> GetFilesByUserNameAsync(string userName)
    {
        var listRequest = new ListObjectsV2Request
        {
            BucketName = _bucketName,
            Prefix = userName
        };

        var response = await _s3Client.ListObjectsV2Async(listRequest);
        
        var r2List = response.S3Objects
            .Select(async obj => new R2File()
            {
                Url = await GeneratePresignedUrl(obj.Key),
                FileName = obj.Key,
            })
            .ToList();

        var r2Files = await Task.WhenAll(r2List);
        
        return AppResponse<List<R2File>>.Success(r2Files.Where(file => !string.IsNullOrEmpty(file.Url)).ToList());
    }
        
    private async Task<string> GeneratePresignedUrl(string fileName)
    {
        AWSConfigsS3.UseSignatureVersion4 = true;
        var presign = new GetPreSignedUrlRequest
        {
            BucketName = _cloudflareConfiguration.BucketName,
            Key = fileName,
            Verb = HttpVerb.GET,
            Expires = DateTime.Now.AddDays(7),
        };

        return await _s3Client.GetPreSignedURLAsync(presign);
    }
}
