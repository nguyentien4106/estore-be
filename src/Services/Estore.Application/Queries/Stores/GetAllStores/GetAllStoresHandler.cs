using BuildingBlocks.Pagination;
using Mapster;
using MediatR;
using System.Linq.Expressions;
using EStore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EStore.Application.Queries.Stores.GetAllStores;

public class GetAllStoresQueryHandler(IEStoreDbContext dbContext) : IRequestHandler<GetAllStoresQuery, GetAllStoresResponse>
{
    public async Task<GetAllStoresResponse> Handle(GetAllStoresQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Store> queryableStores = dbContext.Stores;

        if (!string.IsNullOrWhiteSpace(request.FilterQuery))
        {
            var filter = request.FilterQuery.Trim().ToLowerInvariant();
            queryableStores = queryableStores.Where(s => 
                s.ChannelName != null && s.ChannelName.ToLower().Contains(filter));
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            var isDescending = "DESC".Equals(request.SortOrder, StringComparison.OrdinalIgnoreCase);

            var parameter = Expression.Parameter(typeof(Store), "entity");
            Expression propertyAccess;

            try
            {
                propertyAccess = Expression.Property(parameter, request.SortBy);
            }
            catch (ArgumentException)
            {
                propertyAccess = Expression.Property(parameter, nameof(Store.Id));
            }

            var convertExpression = Expression.Convert(propertyAccess, typeof(object));
            var lambda = Expression.Lambda<Func<Store, object>>(convertExpression, parameter);

            queryableStores = isDescending ? 
                                queryableStores.OrderByDescending(lambda) : 
                                queryableStores.OrderBy(lambda);
        }
        else
        {
            queryableStores = queryableStores.OrderBy(s => s.Id);
        }

        var totalCount = await queryableStores.CountAsync(cancellationToken); 
        var items = await queryableStores.Skip(request.PageIndex * request.PageSize)
                               .Take(request.PageSize)
                               .ToListAsync(cancellationToken);

        var paginationRequest = request.Adapt<PaginationRequest>();

        var paginatedList = new PaginatedResult<StoreDto>(paginationRequest, totalCount, items.Adapt<List<StoreDto>>());

        return new GetAllStoresResponse
        {
            Data = paginatedList,
            Succeed = true
        };
    }
} 