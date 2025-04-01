using Estore.Application.Services;

namespace Estore.Application.Files.Queries.GetImagesByUserName;

public class GetImagesByUserNameHandler(ICloudflareClient client) : IQueryHandler<GetImagesByUserNameQuery, AppResponse<List<R2File>>>
{
    public async Task<AppResponse<List<R2File>>> Handle(GetImagesByUserNameQuery query, CancellationToken cancellationToken)
    {
        return await client.GetImagesByUserNameAsync(query.UserName);
    }
}
