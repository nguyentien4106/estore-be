
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

        var telegramFiles = await context.TeleFileEntities.Where(item => item.UserId == user.Id).OrderByDescending(item => item.CreatedAt).ToListAsync();
        
        var response = telegramFiles.Select(item => new FileEntityResponse(item.Id.ToString(), item.FileName, item.FileSize, item.ContentType, StorageSource.Telegram, item.CreatedAt));

        return AppResponse<List<FileEntityResponse>>.Success(response.ToList());
    }
}