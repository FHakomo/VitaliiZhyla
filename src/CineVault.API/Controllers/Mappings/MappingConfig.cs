using Mapster;
using CineVault.API.Controllers.Requests;
using CineVault.API.Data.Entities;
using CineVault.API.Controllers.Responses;
using CineVault.API.Controllers.Responses.MethodsExclusiveResponses;
namespace CineVault.API.Controllers.Mappings;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Конфігурація для Movie
        config.NewConfig<MovieRequest, Movie>()
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.ReleaseDate, src => src.ReleaseDate)
            .Map(dest => dest.Genre, src => src.Genre)
            .Map(dest => dest.Director, src => src.Director)
            .Map(dest => dest.PosterUrl, src => src.PosterUrl);
        config.NewConfig<Movie, MovieResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.ReleaseDate, src => src.ReleaseDate)
            .Map(dest => dest.Genre, src => src.Genre)
            .Map(dest => dest.Director, src => src.Director)
            .Map(dest => dest.AverageRating, src => src.Reviews.Count != 0 ? src.Reviews.Average(r => r.Rating) : 0)
            .Map(dest => dest.ReviewCount, src => src.Reviews.Count)
            .Map(dest => dest.PosterUrl, src => src.PosterUrl);
        // Конфігурація для User
        config.NewConfig<User, UserResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.AvatarUrl, src => src.AvatarUrl);
        config.NewConfig<UserRequest, User>()
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Password, src => src.Password)
            .Map(dest => dest.AvatarUrl, src => src.AvatarUrl);

        config.NewConfig<ReviewRequest, Review>()
            .Map(dest => dest.Rating, src => src.Rating)
            .Map(dest => dest.Comment, src => src.Comment)
            .Map(dest => dest.MovieId, src => src.MovieId)
            .Map(dest => dest.UserId, src => src.UserId);

        config.NewConfig<Review, ReviewResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.MovieId, src => src.MovieId)
            .Map(dest => dest.MovieTitle, src => src.Movie.Title)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Username, src => src.User.Username)
            .Map(dest => dest.Rating, src => src.Rating)
            .Map(dest => dest.Comment, src => src.Comment)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);
        //Конфігурації для нових DTO
        config.NewConfig<Movie, MovieCreatedResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title);
        // Конфігурація Comment
            config.NewConfig<CommentRequest, Comment>()
                .Map(dest => dest.Message, src => src.Message)
                .Map(dest => dest.ReviewId, src => src.ReviewId)
                .Map(dest => dest.UserId, src => src.UserId);
            config.NewConfig<Comment, CommentResponse>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.ReviewId, src => src.ReviewId)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.Username, src => src.User.Username)
                .Map(dest => dest.Message, src => src.Message)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt);
    }
}
