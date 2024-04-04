using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories.Implementation;
public class MovieRepository(ApplicationDbContext dbContext) : IMovieRepository
{
    public void Create(Movie movie)
    {
        dbContext.Movies.Add(movie);
    }
}
