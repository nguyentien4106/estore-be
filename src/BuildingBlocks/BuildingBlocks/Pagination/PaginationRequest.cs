namespace BuildingBlocks.Pagination;

public class PaginationRequest
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
    public string? FilterQuery { get; set; }

    public PaginationRequest(
        int pageIndex = 0, 
        int pageSize = 10,
        string? sortBy = default,
        string? sortOrder = default,
        string? filterQuery = default)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        SortBy = sortBy;
        SortOrder = sortOrder;
        FilterQuery = filterQuery;
    }
}