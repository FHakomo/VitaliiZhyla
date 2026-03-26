using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Controllers.Responses.MethodsExclusiveResponses;

namespace CineVault.API.Data.Interfaces;

public interface IMovieService
{
     Task<PagedResult<MovieResponse>> SearchMovieAsync(MovieSearchRequest request);
    Task<List<MovieCreatedResponse>> BulkCreateMovies(List<MovieRequest> request);
    Task<List<object?>> BulkDeleteMovies(List<int> ids);
}
