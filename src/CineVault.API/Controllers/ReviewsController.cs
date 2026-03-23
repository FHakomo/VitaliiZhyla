using Asp.Versioning;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineVault.API.Controllers;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public sealed class ReviewsController : ControllerBase
{
    private readonly IReviewRepository reviewRepository;
    private readonly ILogger<ReviewsController> logger;

    public ReviewsController(IReviewRepository reviewRepository, ILogger<ReviewsController> logger)
    {
        this.reviewRepository = reviewRepository;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetReviews()
    {
        var reviews = await this.reviewRepository.GetAllWithDetails();

        var responses = reviews.Select(ReviewResponse.FromEntity);

        logger.LogInformation("Retrieved {ReviewCount} reviews", responses.Count());
        return base.Ok(responses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponse>> GetReviewById(int id)
    {
        var review = await this.reviewRepository.GetByIdWithDetails(id);

        if (review is null)
        {
            logger.LogWarning("Review with ID {ReviewId} not found", id);
            return base.NotFound();
        }

        logger.LogInformation("Retrieved review with ID {ReviewId}", id);
        return base.Ok(ReviewResponse.FromEntity(review));
    }

    [HttpPost]
    public async Task<ActionResult> CreateReview(ReviewRequest request)
    {
        var review = request.ToEntity();

        await this.reviewRepository.Create(review);

        logger.LogInformation("Created new review with ID {ReviewId}", review.Id);
        return base.Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateReview(int id, ReviewRequest request)
    {
        var review = await this.reviewRepository.GetByIdWithDetails(id);

        if (review is null)
        {
            logger.LogWarning("Review with ID {ReviewId} not found for update", id);
            return base.NotFound();
        }

        request.ApplyTo(review);

        await this.reviewRepository.Update(review);

        logger.LogInformation("Updated review with ID {ReviewId}", id);
        return base.Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteReview(int id)
    {
        var review = await this.reviewRepository.GetByIdWithDetails(id);

        if (review is null)
        {
            logger.LogWarning("Review with ID {ReviewId} not found for deletion", id);
            return base.NotFound();
        }

        await this.reviewRepository.Delete(review);
        logger.LogInformation("Deleted review with ID {ReviewId}", id);
        return base.NoContent();
    }
}