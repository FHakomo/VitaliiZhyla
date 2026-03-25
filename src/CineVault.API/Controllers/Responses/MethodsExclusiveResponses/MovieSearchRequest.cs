namespace CineVault.API.Controllers.Responses.MethodsExclusiveResponses;

public class MovieSearchRequest
{
    public string? Title { get; set; }
    public string? Genre { get; set; }
    public string? Director { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public double? MinRating { get; set; }
    public int Page { get; set; } = 1;
    public int? PageSize { get; set; } = 10;



}
