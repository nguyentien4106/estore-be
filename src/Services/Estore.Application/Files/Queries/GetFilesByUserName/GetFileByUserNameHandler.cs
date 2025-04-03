
namespace Estore.Application.Files.Queries.GetFilesByUserName;

public class GetFilesByUserNameHandler(IEStoreDbContext context, UserManager<User> userManager) : IQueryHandler<GetFilesByUserNameQuery, AppResponse<List<FileInformationDto>>>
{
    public async Task<AppResponse<List<FileInformationDto>>> Handle(GetFilesByUserNameQuery query, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(query.UserName);

        if(user is null)
        {
            return AppResponse<List<FileInformationDto>>.NotFound("User", query.UserName);
        }

        var files = await context.R2Files.Where(x => x.UserId == Guid.Parse(user.Id)).ToListAsync(cancellationToken);

        return AppResponse<List<FileInformationDto>>.Success(files.Adapt<List<FileInformationDto>>());
    }
}