﻿using Amazon;
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
            InputStream = FileHelper.GetFileStream(file),
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

    // 📌 Get Image Public URL
    public async Task<AppResponse<R2File>> GetFileByNameAsync(string fileName)
    {
        var url = await GeneratePresignedUrl(fileName);
        return AppResponse<R2File>.Success(new () { Url = url.Data, FileName = fileName });
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
                Url = "",//await GeneratePresignedUrl(obj.Key),
                FileName = obj.Key,
            })
            .ToList();

        var r2Files = await Task.WhenAll(r2List);
        
        return AppResponse<List<R2File>>.Success(r2Files.Where(file => !string.IsNullOrEmpty(file.Url)).ToList());
    }
        
    public async Task<AppResponse<string>> GeneratePresignedUrl(string fileKey)
    {
        AWSConfigsS3.UseSignatureVersion4 = true;
        var presign = new GetPreSignedUrlRequest
        {
            BucketName = _cloudflareConfiguration.BucketName,
            Key = fileKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.Now.AddDays(7),
        };
        var url = await _s3Client.GetPreSignedURLAsync(presign);
        
        return AppResponse<string>.Success(url);
    }
}
