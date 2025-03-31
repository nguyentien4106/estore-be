using Estore.Application.Constants;
using Estore.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Store.Queries.GetImageByFileName;

public class GetImageByFileNameHandler(ICloudflareClient client) : IQueryHandler<GetImageByFileNameQuery, AppResponse<R2File>>
{
    public async Task<AppResponse<R2File>> Handle(GetImageByFileNameQuery query, CancellationToken cancellationToken)
    {
        return await client.GetImageUrlAsync(query.FileName);
    }
}
