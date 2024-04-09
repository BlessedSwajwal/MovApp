using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementation;
public class MovieRepository(ApplicationDbContext dbContext) : IMovieRepository
{
    public async Task Create(Movie movie)
    {
        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Movie>> GetAllAsync()
    {
        var movies = await dbContext.Movies.ToListAsync();
        return movies.AsReadOnly();
    }

    public async Task<IReadOnlyList<Comment>> GetCommentsForAMovie(Guid movieId)
    {
        var comments = await dbContext.Comments.Where(c => c.MovieId == movieId).ToListAsync();
        return comments.AsReadOnly();
    }

    public async Task<Comment?> GetCommentById(Guid id)
    {
        return await dbContext.Comments.SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Movie> GetMovieDetail(Guid movieId)
    {
        var movie = await dbContext.Movies.SingleOrDefaultAsync(m => m.Id == movieId);
        if (movie is null)
        {
            return Movie.Empty;
        }
        return movie;
    }

    public async Task AddComment(Comment comment)
    {
        dbContext.Comments.Add(comment);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteMovie(Guid movieId)
    {
        dbContext.Movies.Remove(await GetMovieDetail(movieId));
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Movie movie)
    {
        dbContext.Update(movie);
        await dbContext.SaveChangesAsync();
    }

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
            {movie.Rating}, 
            {movie.TotalRates}, 
            {movie.Image}");
        var rating = new Ratings(movie.Id, userId, Rating);
        dbContext.Ratings.Add(rating);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> HasUserRatedMovie(Guid movieId, string userId)
    {
        await Task.CompletedTask;
        return dbContext.Ratings.Any(rt => (rt.RatersId == userId && rt.MovieId == movieId));
    }
}
