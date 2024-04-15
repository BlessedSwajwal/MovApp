using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class RatingRepository(ApplicationDbContext dbContext) : IRatingRepository
{
    public async Task AddRating(Movie movie, string userId, int Rating)
    {
        movie.Rating += Rating;
        movie.TotalRates++;
        //dbContext.Update(movie);
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
        $@"EXEC UpdateMovie 
            {movie.Id}, 
            {movie.Name}, 
            {movie.Description}, 
            {movie.ReleaseDate},
            {movie.Rating}, 
            {movie.TotalRates}, 
            {movie.ImagePath}");
        var rating = new Ratings(movie.Id, userId, Rating);
        dbContext.Ratings.Add(rating);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> HasUserRatedMovie(Guid movieId, string userId)
    {
        await Task.CompletedTask;
        return dbContext.Ratings.Any(rt => rt.RatersId == userId && rt.MovieId == movieId);
    }
}
