namespace CineVault.API.Controllers.Responses.MethodsExclusiveResponses;

public sealed class UserStatisticsResponse
{
    public required string Username { get; set; }
    public required int TotalReviews { get; set; }
    public required double AverageRating { get; set; }
     public required List<GenreList> GenreStats { get; set; }
     public required DateTime LastActivity { get; set; }
}
