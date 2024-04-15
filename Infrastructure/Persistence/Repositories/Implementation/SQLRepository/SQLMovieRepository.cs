using Infrastructure.Data;
using Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Implementation.SQLRepository;
public class SQLMovieRepository(ApplicationDbContext dbContext) : IMovieRepository
{

    public Task Create(Movie movie)
    {
        return dbContext.Database.ExecuteSqlInterpolatedAsync(
    $@"EXEC AddMovie 
                {movie.Id}, 
                {movie.Name}, 
                {movie.Description}, 
                {movie.Rating}, 
                {movie.TotalRates}, 
                {movie.ImagePath},
                {movie.ReleaseDate}");
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


    public async Task<Movie> GetMovieDetail(Guid movieId)
    {
        var result = await dbContext.Movies.FromSqlInterpolated($"EXEC GetMovieById {movieId}").ToListAsync();
        var movie = result.FirstOrDefault();
        if (movie is null)
        {
            return Movie.Empty;
        }
        return movie;
    }

    public async Task<IReadOnlyList<Movie>> Search(string searchParam)
    {
        var results = await dbContext.Movies.FromSqlInterpolated($"EXEC SearchMovies {searchParam}").ToListAsync();
        return results;
    }

    public async Task UpdateNameAndDesc(Guid id, string name, string desc, DateOnly releaseDate)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync($"EXEC UpdateMovieDetails {id}, {name}, {desc}, {releaseDate}");
    }

    public async Task UpdateImage(Guid movieId, string imagePath)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync($"EXEC UpdateMovieImage {movieId}, {imagePath}");
    }
}
