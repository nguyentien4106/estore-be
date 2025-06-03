namespace Estore.Application.Models;

public class PaginationQuery 
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
    public string? FilterQuery { get; set; } 
}