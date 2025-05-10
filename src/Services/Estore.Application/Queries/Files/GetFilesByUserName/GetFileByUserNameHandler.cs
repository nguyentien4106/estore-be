using BuildingBlocks.Pagination;
using EStore.Application.Extensions;
using EStore.Application.Models.Files;

namespace EStore.Application.Queries.Files.GetFilesByUserName;

public class GetFilesByUserNameHandler(IEStoreDbContext context, UserManager<User> userManager) 
    : IQueryHandler<GetFilesByUserNameQuery, AppResponse<PaginatedResult<FileEntityResult>>>
{
    public async Task<AppResponse<PaginatedResult<FileEntityResult>>> Handle(GetFilesByUserNameQuery query, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(query.UserName);

        if(user is null)
        {
            return AppResponse<PaginatedResult<FileEntityResult>>.NotFound("User", query.UserName);
        }

        var pageIndex = query.Request.PageIndex;
        var pageSize = query.Request.PageSize;
        var storageSource = query.Request.StorageSource;
        long totalCount = 0;
        List<FileEntityResult> files = [];

        switch (storageSource)
        {
            case StorageSource.R2:
                totalCount = await context.R2FileEntities
                    .Where(item => item.UserId == user.Id)
                    .LongCountAsync(cancellationToken);

                files = await context.R2FileEntities
                    .Where(item => item.UserId == user.Id)
                    .OrderByDescending(item => item.CreatedAt)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .Select(item => item.ToFileEntityResponse())
                    .ToListAsync(cancellationToken);
                break;

            case StorageSource.Telegram:
                totalCount = await context.TeleFileEntities
                    .Where(item => item.UserId == user.Id)
                    .LongCountAsync(cancellationToken);

                files = await context.TeleFileEntities
                    .Where(item => item.UserId == user.Id)
                    .OrderByDescending(item => item.CreatedAt)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .Select(item => item.ToFileEntityResponse())
                    .ToListAsync(cancellationToken);
                break;

            default:
                return AppResponse<PaginatedResult<FileEntityResult>>.Error("Invalid storage source.");
        }

        return AppResponse<PaginatedResult<FileEntityResult>>.Success(
            new PaginatedResult<FileEntityResult>(pageIndex, pageSize, totalCount, files));
    }
}