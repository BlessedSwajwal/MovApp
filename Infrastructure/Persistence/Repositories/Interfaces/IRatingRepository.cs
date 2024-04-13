using Infrastructure.Data;

namespace Infrastructure.Persistence.Repositories.Interfaces;
public interface IRatingRepository
{
    Task AddRating(Movie movie, string userId, int Rating);
    Task<bool> HasUserRatedMovie(Guid movieId, string userId);
}

