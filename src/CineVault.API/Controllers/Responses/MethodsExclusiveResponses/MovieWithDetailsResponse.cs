namespace CineVault.API.Controllers.Responses.MethodsExclusiveResponses;

public sealed class MovieWithDetailsResponse
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public string? Genre { get; set; }
    public string? Director { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<ReviewResponse> Reviews { get; set; } = [];
    public List<string> Actors { get; set; } = [];
}
