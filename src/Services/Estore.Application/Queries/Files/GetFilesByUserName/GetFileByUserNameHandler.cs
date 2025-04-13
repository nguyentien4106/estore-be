using BuildingBlocks.Pagination;
using EStore.Application.Extensions;
using EStore.Application.Models.Files;

namespace EStore.Application.Queries.Files.GetFilesByUserName;

public class GetFilesByUserNameHandler(IEStoreDbContext context, UserManager<User> userManager) : IQueryHandler<GetFilesByUserNameQuery, AppResponse<PaginatedResult<FileEntityResult>>>
{
    public async Task<AppResponse<PaginatedResult<FileEntityResult>>> Handle(GetFilesByUserNameQuery query, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(query.UserName);

        if(user is null)
        {
            return AppResponse<PaginatedResult<FileEntityResult>>.NotFound("User", query.UserName);
        }
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;

        var r2Count = await context.R2FileEntities.Where(item => item.UserId == user.Id).LongCountAsync(cancellationToken);
        var teleCount = await context.TeleFileEntities.Where(item => item.UserId == user.Id).LongCountAsync(cancellationToken);

        var telegramFiles = await context.TeleFileEntities
                                .Where(item => item.UserId == user.Id)
                                .OrderByDescending(item => item.CreatedAt)
                                .Skip(pageIndex * pageSize)
                                .Take(pageSize)
                                .AsNoTracking()
                                .Select(item => item.ToFileEntityResponse())
                                .ToListAsync(cancellationToken);

        var r2Files = await context.R2FileEntities
                                .Where(item => item.UserId == user.Id)
                                .OrderByDescending(item => item.CreatedAt)
                                .Skip(pageIndex * pageSize)
                                .Take(pageSize)
                                .AsNoTracking()
                                .Select(item => item.ToFileEntityResponse())
                                .ToListAsync(cancellationToken);
        
        var response = r2Files.Concat(telegramFiles).OrderByDescending(item => item.CreatedAt).ToList();

        return AppResponse<PaginatedResult<FileEntityResult>>.Success(new PaginatedResult<FileEntityResult>(pageIndex, pageSize, r2Count + teleCount, response) );
    }
}