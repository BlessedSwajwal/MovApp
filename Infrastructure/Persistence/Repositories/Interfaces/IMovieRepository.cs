using Infrastructure.Data;

namespace Infrastructure.Persistence.Repositories.Interfaces;
public interface IMovieRepository
{
    //Task AddComment(Comment comment);
    //Task AddRating(Movie movie, string userId, int Rating);
    public Task Create(Movie movie);
    Task DeleteMovie(Guid movieId);
    public Task<IReadOnlyList<Movie>> GetMovies(int page);

    //public Task<IReadOnlyList<Comment>> GetCommentsForAMovie(Guid movieId);
    Task<Movie> GetMovieDetail(Guid movieId);
    //Task<bool> HasUserRatedMovie(Guid movieId, string userId);
    //Task Update(Movie movie);
    Task<IReadOnlyList<Movie>> Search(string searchParam);
    Task UpdateNameAndDesc(Guid id, string name, string desc, DateOnly releaseDate);
    Task UpdateImage(Guid movieId, string imagePath);
}
