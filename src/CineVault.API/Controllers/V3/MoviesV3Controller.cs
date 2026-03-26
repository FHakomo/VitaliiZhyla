using Asp.Versioning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Data.Interfaces;
using Mapster;
using MapsterMapper;
using CineVault.API.Data.Entities;
using CineVault.API.Controllers.Services;
using CineVault.API.Controllers.Responses.MethodsExclusiveResponses;


namespace CineVault.API.Controllers.V3;

[ApiVersion(3.0)]
[Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]/[action]")]
public class MoviesV3Controller : BaseV3Controller
{
    private readonly IMovieRepository movieRepository;
    private readonly ILogger<MoviesV3Controller> logger;
    private readonly IMapper mapper;
    private readonly MovieService movieService;

    public MoviesV3Controller(IMovieRepository movieRepository, ILogger<MoviesV3Controller> logger, IMapper mapper, MovieService movieService)
    {
        this.movieRepository = movieRepository;
        this.logger = logger;
        this.mapper = mapper;
        this.movieService = movieService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IEnumerable<MovieResponse>>>> GetMovies([FromBody] ApiRequest request)
    {
        this.logger.LogInformation("Received request to get movies with RequestId: {RequestId}", request.RequestId);
        var movies = await this.movieRepository.GetAll();
        var response = this.mapper.Map<IEnumerable<MovieResponse>>(movies);
        this.logger.LogInformation("Retrieved {MoviesCount} movies", response.Count());
        return Ok(response, request.RequestId, "Movies got from Base");
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
            return Ok(this.mapper.Map<MovieResponse>(movie), request.RequestId, $"Movie with id {id} got from Base");
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
            this.mapper.Map(request.Data, movie);
            this.logger.LogInformation("Movie with id {MovieId} found for update", id);
            await this.movieRepository.Update(movie);
            return Ok(this.mapper.Map<MovieResponse>(movie), request.RequestId, $"Movie with id {movie.Id} updated successfully. RequestId = {request.RequestId}");
        }


    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> CreateMovie([FromBody] ApiRequest<MovieRequest> request)
    {
        this.logger.LogInformation("Received request to create movie with RequestId: {RequestId}", request.RequestId);
        var movie = this.mapper.Map<Movie>(request.Data);
        if (movie == null)
        {
            this.logger.LogWarning("Invalid movie data provided for creation. RequestId: {RequestId}", request.RequestId);
            return BadRequest(ApiResponse<MovieResponse>.Fail("Invalid movie data", request.RequestId));
        }
        await this.movieRepository.Create(movie);
        this.logger.LogInformation("Created new movie with ID {MovieId}. RequestId: {RequestId}", movie.Id, request.RequestId);
        return Created(this.mapper.Map<MovieResponse>(movie), request.RequestId, $"Movie with id {movie.Id} created successfully. RequestId = {request.RequestId}");
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
    [HttpPost("bulk")]
    public async Task<ActionResult<ApiResponse<List<MovieCreatedResponse>>>> CreateManyMovies([FromBody] ApiRequest<List<MovieRequest>> request)
    {


        this.logger.LogInformation("Request to create movies in bulk with RequestId: {RequestId}", request.RequestId);
        var created = await movieService.BulkCreateMovies(request.Data);
        return Created(created, request.RequestId, $"Bulk movie creation completed.RequestId: {request.RequestId}");

    }
    [HttpGet("filter")]
    public async Task<ActionResult<ApiResponse<PagedResult<MovieResponse>>>> GetUsersWithFilter(ApiResponse<MovieSearchRequest> request)
    {
        var result = await movieService.SearchAsync(request.Data);
        return Ok(result, request.RequestId, $"Users with filters got successfully. RequestId = {request.RequestId}");
    }
    [HttpDelete("bulk")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteManyMovies([FromBody] ApiRequest<List<int>> request)
    {
        this.logger.LogInformation("Request to delete movies in bulk with RequestId: {RequestId}", request.RequestId);
        await movieService.BulkDeleteMovies(request.Data);
        return Ok<object?>(null, request.RequestId, $"Bulk movie deletion completed.RequestId: {request.RequestId}");
    }
}

