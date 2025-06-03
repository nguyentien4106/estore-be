using Estore.Application.Models;
using MediatR;

namespace EStore.Application.Queries.Stores.GetAllStores;

public class GetAllStoresQuery : PaginationQuery, IRequest<GetAllStoresResponse>
{
} 