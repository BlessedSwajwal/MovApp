using Infrastructure.Data;

namespace Infrastructure.Repositories.Interfaces;
public interface IMovieRepository
{
    public void Create(Movie movie);
}
