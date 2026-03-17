using Asp.Versioning;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineVault.API.Controllers;

[ApiVersion(2.0)]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public sealed class MoviesV2Repository : ControllerBase
{
    private readonly IMovieRepository movieRepository;

    public MoviesV2Repository(IMovieRepository movieRepository)
    {
        this.movieRepository = movieRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieResponse>>> GetMovies()
    {
        var movies = await this.movieRepository.GetAll();

        var response = movies.Select(MovieResponse.FromEntity);

        return base.Ok(new { Version = "V2", Response = response });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MovieResponse>> GetMovieById(int id)
    {
        var movie = await this.movieRepository.GetById(id);

        if (movie is null)
        {
            return base.NotFound();
        }

        return base.Ok(new { Version = "V2", Response = MovieResponse.FromEntity(movie) });
    }

    [HttpPost]
    public async Task<ActionResult> CreateMovie(MovieRequest request)
    {
        var movie = request.ToEntity();

        await this.movieRepository.Create(movie);

        return base.Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateMovie(int id, MovieRequest request)
    {
        var movie = await this.movieRepository.GetById(id);

        if (movie is null)
        {
            return base.NotFound();
        }

        request.ApplyTo(movie);

        await this.movieRepository.Update(movie);

        return base.Ok(new {Version = "V2", Response = movie});
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMovie(int id)
    {
        var movie = await movieRepository.GetById(id);

        if (movie is null)
        {
            return base.NotFound();
        }

        await this.movieRepository.Delete(movie);

        return base.Ok(new {Version = "V2"});
    }
}