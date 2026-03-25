using CineVault.API.Controllers.Requests;
using Mapster;
using CineVault.API.Data.Entities;
using CineVault.API.Data.Interfaces;
using CineVault.API.Controllers.Responses.MethodsExclusiveResponses;

namespace CineVault.API.Controllers.Services;

public class MovieService
{
    private readonly CineVaultDbContext dbContext;
    private readonly ILogger logger;

    public MovieService(ILogger logger, CineVaultDbContext dbContext)
    {
        this.dbContext = dbContext;
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
            await dbContext.Movies.AddAsync(movie);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Created movie with title: {Title}, ID: {MovieId}", movie.Title, movie.Id);
        }
        
        var response = movies.Adapt<List<MovieCreatedResponse>>();
        return response;
    }
}
