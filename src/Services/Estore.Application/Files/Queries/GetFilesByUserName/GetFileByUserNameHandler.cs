using Estore.Application.Services;

namespace Estore.Application.Files.Queries.GetFilesByUserName;

public class GetFilesByUserNameHandler(ICloudflareClient client) : IQueryHandler<GetFilesByUserNameQuery, AppResponse<List<R2File>>>
{
    public async Task<AppResponse<List<R2File>>> Handle(GetFilesByUserNameQuery query, CancellationToken cancellationToken)
    {
        return await client.GetFilesByUserNameAsync(query.UserName);
    }
}
