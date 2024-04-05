using Infrastructure.Data;

namespace Infrastructure.Repositories.Interfaces;
public interface IMovieRepository
{
    void AddComment(Comment comment);
    public void Create(Movie movie);
    public Task<IReadOnlyList<Movie>> GetAllAsync();

    public Task<IReadOnlyList<Comment>> GetCommentsForAMovie(Guid movieId);
    Movie GetMovieDetail(Guid movieId);
    Task SaveAsync();
}
