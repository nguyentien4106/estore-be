using EStore.Application.Services.Cloudflare;
using EStore.Application.Services.R2PresignUrl;
using Microsoft.Extensions.Caching.Memory;

namespace EStore.Application.Queries.Files.GetR2FilePreview;

public class GetR2FilePreviewHandler(
    IR2PresignUrlService service) : IQueryHandler<GetR2FilePreviewQuery, AppResponse<R2PresignUrlResponse>>
{
    public async Task<AppResponse<R2PresignUrlResponse>> Handle(GetR2FilePreviewQuery query, CancellationToken cancellationToken)
    {
        return await service.GetPresignedUrlAsync(query.Id);
    }
} 