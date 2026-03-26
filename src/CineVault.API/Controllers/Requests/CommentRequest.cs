namespace CineVault.API.Controllers.Requests;

public sealed class CommentRequest
{
    public required int ReviewId { get; set; }
    public required int UserId { get; set; }
    public required int Rating { get; set; }
    public string? Message { get; set; }


}
