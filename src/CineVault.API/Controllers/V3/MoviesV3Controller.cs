using Asp.Versioning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Data.Interfaces;


namespace CineVault.API.Controllers.V3;

[ApiVersion("3.0")]
[Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiController]
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

}
