using EStore.Application.Services.Cloudflare;
using EStore.Application.Queries.Files.GetR2FilePreview;

namespace EStore.Application.Services.R2PresignUrl;

public class R2PresignUrlService(ICloudflareClient r2, IEStoreDbContext context) : IR2PresignUrlService
{
    private static readonly TimeSpan UrlExpiration = TimeSpan.FromDays(7);

    public async Task<AppResponse<R2PresignUrlResponse>> GetPresignedUrlAsync(Guid id)
    {
        // Get file entity
        var file = await context.R2FileEntities.FindAsync(id);
        if (file == null)
        {
            return AppResponse<R2PresignUrlResponse>.NotFound("R2File", id.ToString());
        }

        // Generate presigned URL
        var presignedUrl = await r2.GeneratePresignedUrl(file.FileKey);
        if (!presignedUrl.Succeed || string.IsNullOrEmpty(presignedUrl.Data))
        {
            return AppResponse<R2PresignUrlResponse>.Error(presignedUrl.Message);
        }

        // Create response with expiration time
        var response = new R2PresignUrlResponse(
            presignedUrl.Data,
            DateTime.UtcNow.Add(UrlExpiration)
        );

        return AppResponse<R2PresignUrlResponse>.Success(response);
    }
} 