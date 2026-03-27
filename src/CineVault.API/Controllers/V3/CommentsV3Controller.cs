using Asp.Versioning;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Controllers.Responses.MethodsExclusiveResponses;
using CineVault.API.Controllers.Services;
using CineVault.API.Data.Entities;
using CineVault.API.Data.Interfaces;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineVault.API.Controllers.V3;

[ApiVersion(3.0)]
[Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]/[action]")]
public class CommentsV3Controller : BaseV3Controller
{
    private readonly ILogger<CommentsV3Controller> logger;
    private readonly CineVaultDbContext dbContext;
    private readonly IMapper mapper;
    public CommentsV3Controller(ILogger<CommentsV3Controller> logger, CineVaultDbContext dbContext, IMapper mapper)
    {
        this.logger = logger;
        this.dbContext = dbContext;
        this.mapper = mapper;
    }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<IEnumerable<CommentResponse>>>> GetComments([FromBody] ApiRequest request)
    {
        this.logger.LogInformation("Received request to get movies with RequestId: {RequestId}", request.RequestId);
        var comments = await this.dbContext.Comments.Include(c => c.User).Include(c => c.Review).AsNoTracking().ToListAsync();
        var response = this.mapper.Map<IEnumerable<CommentResponse>>(comments);
        this.logger.LogInformation("Retrieved {CommenCount} movies", response.Count());
        return Ok(response, request.RequestId, "Movies got from Base");
    }
    [HttpPost("{id:int}")]
    public async Task<ActionResult<ApiResponse<CommentResponse>>> GetCommentById(int id, [FromBody] ApiRequest request)
    {
        this.logger.LogInformation("Received request to get comment with ID {CommentId} and RequestId: {RequestId}", id, request.RequestId);
        var comment = await this.dbContext.Comments.Include(c => c.User).Include(c => c.Review).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            this.logger.LogWarning("Comment with id {CommentId} not found", id);
            return NotFound(ApiResponse<CommentResponse>.Fail($"Comment with id {id} not found", request.RequestId));
        }
        else
        {
            this.logger.LogInformation("Retrieved comment with ID {CommentId}", id);
            return Ok(this.mapper.Map<CommentResponse>(comment), request.RequestId, $"Comment with id {id} got from Base");
        }
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<CommentResponse>>> UpdateComment(int id, [FromBody] ApiRequest<CommentRequest> request)
    {
        var comment = await this.dbContext.Comments.Include(c => c.User).Include(c => c.Review).FirstOrDefaultAsync(c => c.Id == id);
        this.logger.LogInformation("Received request to update comment with RequestId: {RequestId}", request.RequestId);
        if (comment == null)
        {
            this.logger.LogWarning("Comment with id {CommentId} not found for update", id);
            return NotFound(ApiResponse<CommentResponse>.Fail($"Comment with id {id} not found", request.RequestId));
        }
        else
        {
            this.mapper.Map(request.Data, comment);
            this.logger.LogInformation("Comment with id {CommentId} found for update", id);
            await this.dbContext.SaveChangesAsync();
            return Ok(this.mapper.Map<CommentResponse>(comment), request.RequestId, $"Comment with id {comment.Id} updated successfully. RequestId = {request.RequestId}");
        }
    }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CommentResponse>>> CreateComment([FromBody] ApiRequest<CommentRequest> request)
    {
        this.logger.LogInformation("Received request to create comment with RequestId: {RequestId}", request.RequestId);
        var comment = this.mapper.Map<Comment>(request.Data);
        await this.dbContext.Comments.AddAsync(comment);
        await this.dbContext.SaveChangesAsync();
        return Ok(this.mapper.Map<CommentResponse>(comment), request.RequestId, $"Comment with id {comment.Id} created successfully. RequestId = {request.RequestId}");
    }
    [HttpPost("{id:int}/delete")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteComment(int id, [FromBody] ApiRequest<object?> request)
    {
        var comment = await this.dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
        this.logger.LogInformation("Received request to delete comment with RequestId: {RequestId}", request.RequestId);
        if (comment == null)
        {
            this.logger.LogWarning("Comment with id {CommentId} not found for deletion", id);
            return NotFound(ApiResponse<object?>.Fail($"Comment with id {id} not found", request.RequestId));
        }
        else
        {
            comment.IsDeleted = true;
            await this.dbContext.SaveChangesAsync();
            return Ok<object?>(null!,($"Comment with id {id} deleted successfully. RequestId = {request.RequestId}"));
        }
    }
    [HttpPost("comment/{commentId:int}/like")]
    public async Task<ActionResult<ApiResponse<object?>>> LikeComment(int commentId, [FromQuery] int userId, [FromQuery] string requestId)
    {
        var comment = await this.dbContext.Comments.Include(c => c.Likes).FirstOrDefaultAsync(c => c.Id == commentId);
        if (comment == null)
        {
            this.logger.LogWarning("Comment with id {CommentId} not found for like", commentId);
            return NotFound(ApiResponse<object?>.Fail($"Comment with id {commentId} not found", requestId));
        }
        else
        {
            var existingLike = comment.Likes.FirstOrDefault(l => l.UserId == userId);
            if (existingLike != null)
            {

                 this.logger.LogInformation("User with id {UserId} already liked comment with id {CommentId}", userId, commentId);
                return Ok(new ApiResponse<object?> { Success = true, Message = "Comment already liked", RequestId = requestId});

            }
            else
            {
                var like = new CommentLike { UserId = userId, CommentId = commentId };
                comment.Likes.Add(like);
                await this.dbContext.CommentLikes.AddAsync(like);
                await this.dbContext.SaveChangesAsync();
                return Ok(new ApiResponse<object?> { Success = true, Message = "Comment successfully liked", RequestId = requestId });
            }

        }
    }
}
