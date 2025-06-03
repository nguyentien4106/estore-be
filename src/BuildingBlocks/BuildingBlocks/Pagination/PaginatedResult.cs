namespace BuildingBlocks.Pagination;

public class PaginatedResult<TEntity> where TEntity : class
{
    public int PageIndex { get; } 
    
    public int PageSize { get; } 

    public string? SortBy { get; }
    
    public string? SortOrder { get; }
    
    public string? FilterQuery { get; }

    public long Count { get; } 

    public IEnumerable<TEntity> Data { get; } 

    public int TotalPages => (int)Math.Ceiling(Count / (double)PageSize);

    public bool HasPreviousPage => PageIndex > 0;

    public bool HasNextPage => PageIndex < TotalPages;
    
    public PaginatedResult(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Count = count;
        Data = data;
    }

    public PaginatedResult(PaginationRequest paginationRequest, long count, IEnumerable<TEntity> data)
    {
        PageIndex = paginationRequest.PageIndex;
        PageSize = paginationRequest.PageSize;
        SortBy = paginationRequest.SortBy;
        SortOrder = paginationRequest.SortOrder;
        FilterQuery = paginationRequest.FilterQuery;
        Count = count;
        Data = data;
    }
}