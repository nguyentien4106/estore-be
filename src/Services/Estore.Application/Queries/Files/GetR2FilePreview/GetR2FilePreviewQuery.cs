namespace EStore.Application.Queries.Files.GetR2FilePreview;

public record GetR2FilePreviewQuery(Guid Id) : IQuery<AppResponse<R2PresignUrlResponse>>;
