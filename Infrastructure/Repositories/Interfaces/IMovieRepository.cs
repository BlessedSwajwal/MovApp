using Infrastructure.Data;

namespace Infrastructure.Repositories.Interfaces;
public interface IMovieRepository
{
    Task AddComment(Comment comment);
    Task AddRating(Movie movie, string userId, int Rating);
    public Task Create(Movie movie);
    Task DeleteMovie(Guid movieId);
    public Task<IReadOnlyList<Movie>> GetMovies(int page);

    public Task<IReadOnlyList<Comment>> GetCommentsForAMovie(Guid movieId);
    Task<Movie> GetMovieDetail(Guid movieId);
    Task<bool> HasUserRatedMovie(Guid movieId, string userId);
    Task Update(Movie movie);
}
