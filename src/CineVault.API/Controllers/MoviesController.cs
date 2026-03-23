using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CineVault.API.Controllers;

[Route("api/[controller]/[action]")]

public sealed class MoviesController : ControllerBase
{
    private readonly IMovieRepository movieRepository;
    private readonly ILogger<MoviesController> logger;

    public MoviesController(IMovieRepository movieRepository, ILogger<MoviesController> logger)
    {
        this.movieRepository = movieRepository;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieResponse>>> GetMovies()
    {
        var movies = await this.movieRepository.GetAll();

        var response = movies.Select(MovieResponse.FromEntity);

        logger.LogInformation("Retrieved {MoviesCount} movies", response.Count());
        return base.Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MovieResponse>> GetMovieById(int id)
    {
        var movie = await this.movieRepository.GetById(id);

        if (movie is null)
        {
            logger.LogWarning("Movie with ID {MovieId} not found", id);
            return base.NotFound();
        }

        logger.LogInformation("Retrieved movie with ID {MovieId}", id);
        return base.Ok(MovieResponse.FromEntity(movie));
    }

    [HttpPost]
    public async Task<ActionResult> CreateMovie(MovieRequest request)
    {
        var movie = request.ToEntity();

        await this.movieRepository.Create(movie);

        logger.LogInformation("Created new movie with ID {MovieId}", movie.Id);
        return base.Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateMovie(int id, MovieRequest request)
    {
        var movie = await this.movieRepository.GetById(id);

        if (movie is null)
        {
            logger.LogWarning("Movie with ID {MovieId} not found for update", id);
            return base.NotFound();
        }

        request.ApplyTo(movie);

        await this.movieRepository.Update(movie);
        logger.LogInformation("Updated movie with ID {MovieId}", id);
        return base.Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMovie(int id)
    {
        var movie = await movieRepository.GetById(id);

        if (movie is null)
        {
            logger.LogWarning("Movie with ID {MovieId} not found for deletion", id);
            return base.NotFound();
        }

        await this.movieRepository.Delete(movie);

        logger.LogInformation("Deleted movie with ID {MovieId}", id);
        return base.NoContent();
    }
}