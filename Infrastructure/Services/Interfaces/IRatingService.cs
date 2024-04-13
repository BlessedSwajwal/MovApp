using Infrastructure.DTOs.Movie;

namespace Infrastructure.Services.Interfaces;
public interface IRatingService
{
    Task AddRating(MovieDetailDTO movieDto, string userId, int Rating);
    Task<bool> HasUserAlreadyRated(Guid movieId, string userId);
}
