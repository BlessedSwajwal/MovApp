using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Repositories.SQLImplementation;
public class SQLMovieRepository(ApplicationDbContext dbContext) : IMovieRepository
{
    public async Task AddComment(Comment comment)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync($"EXEC AddComments {comment.Id}, {comment.CommenterId}, {comment.MovieId}, {comment.CommenterName}, {comment.Description}");
    }

    public async Task AddRating(Movie movie, string userId, int Rating)
    {
        movie.Rating += Rating;
        movie.TotalRates++;
        await Update(movie);

        await dbContext.Database.ExecuteSqlInterpolatedAsync($"EXEC AddRating {Guid.NewGuid()}, {movie.Id}, {userId}, {Rating}");
    }

    public Task Create(Movie movie)
    {
        return dbContext.Database.ExecuteSqlInterpolatedAsync(
    $@"EXEC AddMovie 
                {movie.Id}, 
                {movie.Name}, 
                {movie.Description}, 
                {movie.Rating}, 
                {movie.TotalRates}, 
                {movie.Image},
                {movie.ReleaseDate}");
    }

    public Task Delete(Guid movieId)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteMovie(Guid movieId)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync($"EXEC RemoveMovie {movieId}");
    }

    public async Task<IReadOnlyList<Movie>> GetMovies(int page = 1)
    {
        var movies = await dbContext.Movies.FromSqlRaw("EXEC GetPagedMoviesByRating @PageNumber, @PageSize", new SqlParameter("@PageNumber", page), new SqlParameter("@PageSize", 10)).ToListAsync();
        Console.WriteLine(movies);
        return movies;
    }

    public async Task<IReadOnlyList<Comment>> GetCommentsForAMovie(Guid movieId)
    {
        var comments = await dbContext.Comments.FromSqlInterpolated(
    $@"EXEC GetCommentsForAMovie
                    {movieId}").ToListAsync();
        return comments;
    }

    public async Task<Movie> GetMovieDetail(Guid movieId)
    {
        //var result = await dbContext.Movies.FromSqlInterpolated($"EXEC GetMovieById {movieId}").ToListAsync();
        //var movie = result.FirstOrDefault();
        //if (movie is null)
        //{
        //    return Movie.Empty;
        //}
        //return movie;

        var result = await dbContext.Movies.FromSqlInterpolated($"EXEC GetMovieById {movieId}").ToListAsync();
        var movie = result.FirstOrDefault();
        if (movie is null)
        {
            return Movie.Empty;
        }
        return movie;
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

    //public Task Update(Movie movie)
    //{
    //    throw new NotImplementedException();
    //}


    public async Task Update(Movie movie)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
        $@"EXEC UpdateMovie 
            {movie.Id}, 
            {movie.Name}, 
            {movie.Description}, 
            {movie.Rating}, 
            {movie.TotalRates}, 
            {movie.Image},
            {movie.ReleaseDate}");
    }

    public async Task<IReadOnlyList<Movie>> Search(string searchParam)
    {
        var results = await dbContext.Movies.FromSqlInterpolated($"EXEC SearchMovies {searchParam}").ToListAsync();
        return results;
    }
}
