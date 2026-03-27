namespace CineVault.API.Data.Entities;

public sealed class MovieActor
{
    public int Id { get; set; }
    public required int MovieId { get; set; }
    public Movie? Movie { get; set; }
    public required int ActorId { get; set; }
    public Actor? Actor { get; set; }
}
