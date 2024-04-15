using Infrastructure.Data;
using Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Implementation.LINQRepository;
public class MovieRepository(ApplicationDbContext dbContext) : IMovieRepository
{
    public async Task Create(Movie movie)
    {
        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Movie>> GetMovies(int page = 0)
    {
        var movies = await dbContext.Movies.OrderByDescending(m => m.TotalRates).Skip(page * 10).Take(10).ToListAsync();
        return movies.AsReadOnly();
    }

    public async Task UpdateNameAndDesc(Guid id, string name, string desc, DateOnly releaseDate)
    {
        var movieToUpdate = new Movie { Id = id, Name = name, Description = desc, ReleaseDate = releaseDate };

        // Attach the entity with modified properties
        dbContext.Attach(movieToUpdate);

        // Explicitly mark the properties as modified
        dbContext.Entry(movieToUpdate).Property(p => p.Name).IsModified = true;
        dbContext.Entry(movieToUpdate).Property(p => p.Description).IsModified = true;
        dbContext.Entry(movieToUpdate).Property(p => p.ReleaseDate).IsModified = true;

        // Save changes without full entity retrieval
        await dbContext.SaveChangesAsync();
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


    public async Task DeleteMovie(Guid movieId)
    {

        var movie = await GetMovieDetail(movieId);
        dbContext.Movies.Remove(movie);
        await dbContext.SaveChangesAsync();
    }



    public async Task<IReadOnlyList<Movie>> Search(string searchParam)
    {
        var movies = await dbContext.Movies.Where(m => m.Name.ToLower().Contains(searchParam.ToLower())).ToListAsync();
        return movies;
    }
}
