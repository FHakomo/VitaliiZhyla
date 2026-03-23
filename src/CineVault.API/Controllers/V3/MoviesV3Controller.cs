using Asp.Versioning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Data.Interfaces;


namespace CineVault.API.Controllers.V3;

[ApiVersion(3.0)]
[Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]/[action]")]
public class MoviesV3Controller : BaseV3Controller
{
    private readonly IMovieRepository movieRepository;
    private readonly ILogger<MoviesV3Controller> logger;

    public MoviesV3Controller(IMovieRepository movieRepository, ILogger<MoviesV3Controller> logger)
    {
        this.movieRepository = movieRepository;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IEnumerable<MovieResponse>>>> GetMovies([FromBody] ApiRequest request)
    {
        this.logger.LogInformation("Received request to get movies with RequestId: {RequestId}", request.RequestId);
        var movies = await this.movieRepository.GetAll();
        var response = movies.Select(MovieResponse.FromEntity);
        this.logger.LogInformation("Retrieved {MoviesCount} movies", response.Count());
        return Ok(response, request.RequestId, "Movies got from Base" );
    }
    [HttpPost("{id:int}")]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> GetMovieById(int id, [FromBody] ApiRequest request)
    {
        this.logger.LogInformation("Received request to get movie with ID {MovieId} and RequestId: {RequestId}", id, request.RequestId);
        var movie = await this.movieRepository.GetById(id);
        if (movie == null)
        {
            this.logger.LogWarning("Movie with id {MovieId} not found", id);
            return NotFound(ApiResponse<MovieResponse>.Fail($"Movie with id {id} not found", request.RequestId));
        }
        else
        {
            this.logger.LogInformation("Retrieved movie with ID {MovieId}", id);
            return Ok(MovieResponse.FromEntity(movie), request.RequestId, $"Movie with id {id} got from Base");
        }
    }   
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> UpdateMovie(int id, [FromBody] ApiRequest<MovieRequest> request)
    {
        var movie = await this.movieRepository.GetById(id);
        this.logger.LogInformation("Received request to update movie with RequestId: {RequestId}", request.RequestId);

        if (movie == null)
        {
            this.logger.LogWarning("Movie with id {MovieId} not found for update", id);
            return NotFound(ApiResponse<MovieResponse>.Fail($"Movie with id {id} not found", request.RequestId));
        }
        else
        {
            request.Data?.ApplyTo(movie);
            { this.logger.LogInformation("Movie with id {MovieId} found for update", id); }
            await this.movieRepository.Update(movie);
            return Ok(MovieResponse.FromEntity(movie), request.RequestId, $"Movie with id {movie.Id} updated successfully. RequestId = {request.RequestId}");
        }
            

    }

    [HttpPost("{id:int}")]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> CreateMovie([FromBody] ApiRequest<MovieRequest> request)
    {
        this.logger.LogInformation("Received request to create movie with RequestId: {RequestId}", request.RequestId);
        var movie = request.Data?.ToEntity();
        if (movie == null)
        {
            this.logger.LogWarning("Invalid movie data provided for creation. RequestId: {RequestId}", request.RequestId);
            return BadRequest(ApiResponse<MovieResponse>.Fail("Invalid movie data", request.RequestId));
        }
        await this.movieRepository.Create(movie);
        this.logger.LogInformation("Created new movie with ID {MovieId}. RequestId: {RequestId}", movie.Id, request.RequestId);
        return Ok(MovieResponse.FromEntity(movie), request.RequestId, $"Movie with id {movie.Id} created successfully. RequestId = {request.RequestId}");
    }
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteMovie(
        int id, [FromBody] ApiRequest<object?> request)
    {
        logger.LogInformation("Deleting movie {MovieId} (v3). RequestId: {RequestId}", id, request.RequestId);
        var movie = await movieRepository.GetById(id);
        if (movie is null)
        {
            return NotFound(ApiResponse<object?>.Fail(request.RequestId, $"Movie with id {id} not found"));
        }
        await movieRepository.Delete(movie);
        return Ok<object?>(null, request.RequestId, "Movie deleted successfully");
    }
}

