using CineVault.API.Data.Entities;

namespace CineVault.API.Controllers.Responses;

public sealed class CommentResponse
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public required int Rating { get; set; }
    public required int ReviewId { get; set; }
    public required int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Username { get; set; }
}
