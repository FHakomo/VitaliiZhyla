using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Controllers.Responses.MethodsExclusiveResponses;
using CineVault.API.Data.Entities;
using CineVault.API.Data.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CineVault.API.Controllers.Services;

public class MovieService : IMovieService
{
    private readonly CineVaultDbContext dbContext;
    private readonly ILogger<MovieService> logger;   // ← змінено

    public MovieService(ILogger<MovieService> logger, CineVaultDbContext dbContext)   // ← змінено
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

    public async Task<PagedResult<MovieResponse>> SearchMovieAsync(MovieSearchRequest request)
    {
        var query = dbContext.Movies.AsQueryable();
        if (request.Title != null)
        {
            query = query.Where(m => m.Title.Contains(request.Title));
        }
        if (request.Genre != null)
        {
            query = query.Where(m => m.Genre.Contains(request.Genre));
        }
        if (request.Director != null)
        {
            query = query.Where(m => m.Director.Contains(request.Director));
        }
        if (request.YearFrom.HasValue)
        {
            query = query.Where(m => m.ReleaseDate.Value.Year >= request.YearFrom);
        }
        if (request.YearTo.HasValue)
        {
            query = query.Where(m => m.ReleaseDate.Value.Year <= request.YearTo);
        }
        if (request.MinRating.HasValue)
            query = query.Where(m => m.Reviews.Average(r => r.Rating) >= request.MinRating);

        var total = await query.CountAsync();
        var items = await query.Skip((request.Page - 1) * request.PageSize.Value).Take(request.PageSize.Value).ToListAsync();
        logger.LogInformation("Movie search with {Filters}, total {Total}", request, total);
        return new PagedResult<MovieResponse>
        {
            Items = items.Adapt<List<MovieResponse>>(),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize ?? total
        };
    }

    public async Task<List<object?>> BulkDeleteMovies(List<int> ids)
    {
        var movies = await dbContext.Movies.Where(m => ids.Contains(m.Id)).ToListAsync();
        if (movies.Count == 0)
        {
            logger.LogWarning("No movies found for bulk deletion with IDs: {Ids}", string.Join(", ", ids));
            return new List<object?>();
        }
        foreach (Movie movie in movies)
        {
            if (movie.Reviews.Any())
            {
                logger.LogWarning("Skipping deletion of movie with ID: {MovieId} due to existing reviews.", movie.Id);
                continue;
            }
            movie.IsDeleted = true;
            logger.LogInformation("Deleted movie with ID: {MovieId}", movie.Id);
        }
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Deleted movies with IDs: {Ids}", string.Join(", ", movies.Select(m => m.Id)));
        return movies.Select(m => (object?)null).ToList();
    }
}