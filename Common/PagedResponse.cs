namespace TaskFlow.Common;

/// <summary>Wraps a paginated list result so the client gets both the current page of items and the metadata needed to build pagination UI (total count, current page, total pages).</summary>
public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}