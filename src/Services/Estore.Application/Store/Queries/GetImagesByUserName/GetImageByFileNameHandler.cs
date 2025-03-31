using Estore.Application.Constants;
using Estore.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Store.Queries.GetImagesByUserName;

public class GetImagesByUserNameHandler(ICloudflareClient client) : IQueryHandler<GetImagesByUserNameQuery, AppResponse<List<R2File>>>
{
    public async Task<AppResponse<List<R2File>>> Handle(GetImagesByUserNameQuery query, CancellationToken cancellationToken)
    {
        return await client.GetImagesByUserNameAsync(query.UserName);
    }
}
