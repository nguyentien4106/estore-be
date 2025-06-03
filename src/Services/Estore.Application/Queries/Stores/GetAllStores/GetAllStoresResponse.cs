using BuildingBlocks.Pagination;

namespace EStore.Application.Queries.Stores.GetAllStores;

public class GetAllStoresResponse : AppResponse<PaginatedResult<StoreDto>>
{
}


public class StoreDto 
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public string ChannelName { get; set; }

    public DateTime CreatedDate { get; set; }

    public long ChannelId { get; set; }

    public string? Description { get; set; }


    public long MessageCount { get; set; }
} 