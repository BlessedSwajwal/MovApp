using Infrastructure.Data;
using Infrastructure.DTOs.Movie;
using Infrastructure.Persistence.Repositories.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services.Implementation;



public class RatingService(IRatingRepository _ratingRepository) : IRatingService
{
    public async Task AddRating(MovieDetailDTO movieDto, string userId, int Rating)
    {
        var movie = Movie.Create(movieDto.Id, movieDto.Name, movieDto.Description, movieDto.Image, movieDto.Rating, movieDto.TotalRates, new List<Guid>(), movieDto.ReleaseDate);
        await _ratingRepository.AddRating(movie, userId, Rating);
    }

    public async Task<bool> HasUserAlreadyRated(Guid movieId, string userId)
    {
        var hasRated = await _ratingRepository.HasUserRatedMovie(movieId, userId);
        return hasRated;
    }
}
