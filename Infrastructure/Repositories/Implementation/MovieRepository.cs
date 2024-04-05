using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementation;
public class MovieRepository(ApplicationDbContext dbContext) : IMovieRepository
{
    public async Task SaveAsync()
    {
        await dbContext.SaveChangesAsync();
    }
    public void Create(Movie movie)
    {
        dbContext.Movies.Add(movie);
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

    public Comment? GetCommentById(Guid id)
    {
        return dbContext.Comments.SingleOrDefault(c => c.Id == id);
    }

    public Movie GetMovieDetail(Guid movieId)
    {
        var movie = dbContext.Movies.SingleOrDefault(m => m.Id == movieId);
        if (movie is null)
        {
            return Movie.Empty;
        }
        return movie;
    }

    public void AddComment(Comment comment)
    {
        dbContext.Comments.Add(comment);
        dbContext.SaveChanges();
    }

    public void DeleteMovie(Guid movieId)
    {
        dbContext.Movies.Remove(GetMovieDetail(movieId));
        dbContext.SaveChanges();
    }
}
