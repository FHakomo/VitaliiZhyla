using Asp.Versioning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Data.Interfaces;


namespace CineVault.API.Controllers.V3;

[ApiVersion(3.0)]
[Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]/[action]")]
public class ReviewsV3Controller : BaseV3Controller
{
    private readonly IReviewRepository reviewRepository;
    private readonly ILogger<ReviewsV3Controller> logger;

    public ReviewsV3Controller(IReviewRepository reviewRepository, ILogger<ReviewsV3Controller> logger)
    {
        this.reviewRepository = reviewRepository;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReviewResponse>>>> GetReviews([FromBody] ApiRequest request)
    {
        this.logger.LogInformation("Received request to get reviews with RequestId: {RequestId}", request.RequestId);
        var reviews = await this.reviewRepository.GetAllWithDetails();
        var response = reviews.Select(ReviewResponse.FromEntity);
        this.logger.LogInformation("Retrieved {ReviewCount} reviews", response.Count());
        return Ok(response, request.RequestId, "Reviews got from Base" );
    }
    [HttpPost("{id:int}")]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> GetReviewById(int id, [FromBody] ApiRequest request)
    {
        this.logger.LogInformation("Received request to get review with ID {ReviewId} and RequestId: {RequestId}", id, request.RequestId);
        var review = await this.reviewRepository.GetByIdWithDetails(id);
        if (review == null)
        {
            this.logger.LogWarning("Review with id {ReviewId} not found", id);
            return NotFound(ApiResponse<ReviewResponse>.Fail($"Review with id {id} not found", request.RequestId));
        }
        else
        {
            this.logger.LogInformation("Retrieved review with ID {ReviewId}", id);
            return Ok(ReviewResponse.FromEntity(review), request.RequestId, $"Review with id {id} got from Base");
        }
    }   
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> UpdateReview(int id, [FromBody] ApiRequest<ReviewRequest> request)
    {
        var review = await this.reviewRepository.GetByIdWithDetails(id);
        this.logger.LogInformation("Received request to update review with RequestId: {RequestId}", request.RequestId);

        if (review == null)
        {
            this.logger.LogWarning("Review with id {ReviewId} not found for update", id);
            return NotFound(ApiResponse<ReviewResponse>.Fail($"Review with id {id} not found", request.RequestId));
        }
        else
        {
            request.Data?.ApplyTo(review);
            { this.logger.LogInformation("Review with id {ReviewId} found for update", id); }
            await this.reviewRepository.Update(review);
            return Ok(ReviewResponse.FromEntity(review), request.RequestId, $"Review with id {review.Id} updated successfully. RequestId = {request.RequestId}");
        }
            

    }

    [HttpPost("{id:int}")]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> CreateReview([FromBody] ApiRequest<ReviewRequest> request)
    {
        this.logger.LogInformation("Received request to create review with RequestId: {RequestId}", request.RequestId);
        var review = request.Data?.ToEntity();
        if (review == null)
        {
            this.logger.LogWarning("Invalid review data provided for creation. RequestId: {RequestId}", request.RequestId);
            return BadRequest(ApiResponse<ReviewResponse>.Fail("Invalid review data", request.RequestId));
        }
        await this.reviewRepository.Create(review);
        this.logger.LogInformation("Created new review with ID {ReviewId}. RequestId: {RequestId}", review.Id, request.RequestId);
        return Ok(ReviewResponse.FromEntity(review), request.RequestId, $"Review with id {review.Id} created successfully. RequestId = {request.RequestId}");
    }
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteReview(
        int id, [FromBody] ApiRequest<object?> request)
    {
        logger.LogInformation("Deleting review {ReviewId} (v3). RequestId: {RequestId}", id, request.RequestId);
        var review = await reviewRepository.GetByIdWithDetails(id);
        if (review is null)
        {
            return NotFound(ApiResponse<object?>.Fail(request.RequestId, $"Review with id {id} not found"));
        }
        await reviewRepository.Delete(review);
        return Ok<object?>(null, request.RequestId, "Review deleted successfully");
    }
}

