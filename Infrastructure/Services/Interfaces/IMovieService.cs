using Infrastructure.Data;
using Infrastructure.DTOs.Movie;

namespace Infrastructure.Services.Interfaces;
public interface IMovieService
{
    Task AddRating(MovieDetailDTO movieDto, string userId, int Rating);
    public Task<Movie> CreateMovieAsync(CreateMovieDTO createMovieDTO);
    void DeleteMovie(Guid movieId);
    Task<byte[]> FetchImageAsync(string imageUrl);
    Task<IReadOnlyList<MovieListDTO>> GetAllMovieAsync();
    Task<MovieDetailDTO> GetMovieDetail(Guid movieId);
    Task<List<TrendingMovieDTO>> GetTrendingMovies(int page = 1);
    Task<bool> HasUserAlreadyRated(Guid movieId, string userId);
    Task PostComment(string commentText, Guid movieId, Guid commenterId, string commenterName);
    Task Update(MovieDetailDTO updatedMovie);
}
