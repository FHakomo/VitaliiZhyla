namespace CineVault.API.Data.Entities;

public sealed class Comment
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public required int Rating { get; set; }
    public required int ReviewId { get; set; }
    public required int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Review? Review { get; set; }
    public User? User { get; set; }
    public ICollection<CommentLike> Likes { get; set; } = [];

}
