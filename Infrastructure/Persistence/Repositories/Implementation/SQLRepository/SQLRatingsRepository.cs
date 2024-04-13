using Infrastructure.Data;
using Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Persistence.Repositories.Implementation.SQLRepository;
public class SQLRatingsRepository(ApplicationDbContext dbContext, IMovieRepository movieRepository) : IRatingRepository
{
    public async Task AddRating(Movie movie, string userId, int Rating)
    {
        movie.Rating += Rating;
        movie.TotalRates++;
        await movieRepository.Update(movie);

        await dbContext.Database.ExecuteSqlInterpolatedAsync($"EXEC AddRating {Guid.NewGuid()}, {movie.Id}, {userId}, {Rating}");
    }

    public async Task<bool> HasUserRatedMovie(Guid movieId, string userId)
    {
        // Define output parameter
        var hasRatedParam = new SqlParameter("@HasRated", SqlDbType.Bit)
        {
            Direction = ParameterDirection.Output
        };

        // Execute stored procedure
        await dbContext.Database.ExecuteSqlRawAsync(
            "EXEC CheckUserMovieRating @MovieId, @UserId, @HasRated OUTPUT",
            new SqlParameter("@MovieId", movieId),
            new SqlParameter("@UserId", userId),
            hasRatedParam);

        // Retrieve value of output parameter
        bool hasRated = (bool)hasRatedParam.Value;

        return hasRated;
    }
}
