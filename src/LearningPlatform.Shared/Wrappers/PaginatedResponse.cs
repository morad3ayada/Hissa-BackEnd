using LearningPlatform.Shared.Pagination;

namespace LearningPlatform.Shared.Wrappers;

public class PaginatedResponse<T> : ApiResponse<List<T>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }

    public static PaginatedResponse<T> Create(PaginatedList<T> paginatedList, string? message = null) =>
        new()
        {
            Succeeded = true,
            Message = message,
            Data = paginatedList.Items,
            PageNumber = paginatedList.PageNumber,
            PageSize = paginatedList.PageSize,
            TotalPages = paginatedList.TotalPages,
            TotalCount = paginatedList.TotalCount,
            HasPreviousPage = paginatedList.HasPreviousPage,
            HasNextPage = paginatedList.HasNextPage
        };
}
