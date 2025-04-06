
using EStore.Application.Extensions;

namespace Estore.Application.Files.Queries.GetFilesByUserName;

public class GetFilesByUserNameHandler(IEStoreDbContext context, UserManager<User> userManager) : IQueryHandler<GetFilesByUserNameQuery, AppResponse<List<FileEntityResponse>>>
{
    public async Task<AppResponse<List<FileEntityResponse>>> Handle(GetFilesByUserNameQuery query, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(query.UserName);

        if(user is null)
        {
            return AppResponse<List<FileEntityResponse>>.NotFound("User", query.UserName);
        }

        var telegramFiles = await context.TeleFileEntities.Where(item => item.UserId == user.Id).Select(item => item.ToFileEntityResponse()).ToListAsync(cancellationToken);
        var r2Files = await context.R2FileEntities.Where(item => item.UserId == user.Id).Select(item => item.ToFileEntityResponse()).ToListAsync(cancellationToken);
        
        var response = r2Files.Concat(telegramFiles).OrderByDescending(item => item.CreatedAt).ToList();

        return AppResponse<List<FileEntityResponse>>.Success(response);
    }
}