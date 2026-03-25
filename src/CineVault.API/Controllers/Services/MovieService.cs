using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using Mapster;
using CineVault.API.Data.Entities;
using CineVault.API.Data.Interfaces;

namespace CineVault.API.Controllers.Services;

public class MovieService
{
    private readonly IMovieRepository movieRepository;
    private readonly ILogger logger;

    public MovieService(IMovieRepository movieRepository, ILogger logger)
    {
        this.movieRepository = movieRepository;
        this.logger = logger;
    }
    public async Task<List<MovieCreatedResponse>> BulkCreateMovies(List<MovieRequest> request)
    {
        var movies = request.Adapt<List<Movie>>();
        foreach (var movie in movies)
        {
            if (string.IsNullOrWhiteSpace(movie.Title))
            {
                logger.LogWarning("Skipping movie creation due to missing title.");
                continue;
            }
            await this.movieRepository.Create(movie);
            logger.LogInformation("Created movie with title: {Title}, ID: {MovieId}", movie.Title, movie.Id);
        }
        
        var response = movies.Adapt<List<MovieCreatedResponse>>();
        return response;
    }
}
