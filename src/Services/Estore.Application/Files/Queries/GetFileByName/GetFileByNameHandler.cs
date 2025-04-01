using Estore.Application.Services;

namespace Estore.Application.Files.Queries.GetFileByName;

public class GetFileByNameHandler(ICloudflareClient client) : IQueryHandler<GetFileByNameQuery, AppResponse<R2File>>
{
    public async Task<AppResponse<R2File>> Handle(GetFileByNameQuery query, CancellationToken cancellationToken)
    {
        return await client.GetFileByNameAsync(query.FileName);
    }
}
