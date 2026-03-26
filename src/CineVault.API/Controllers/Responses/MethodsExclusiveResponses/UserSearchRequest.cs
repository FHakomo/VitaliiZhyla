namespace CineVault.API.Controllers.Responses.MethodsExclusiveResponses;

public class UserSearchRequest
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string SortBy { get; set; } = "username";
    public int Page { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
