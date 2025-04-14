using EStore.Application.Queries.Files.GetR2FilePreview;

namespace EStore.Application.Services.R2PresignUrl;

public interface IR2PresignUrlService
{
    Task<AppResponse<R2PresignUrlResponse>> GetPresignedUrlAsync(Guid id);
}
