using Infrastructure.Data;
using Infrastructure.DTOs.Movie;

namespace Infrastructure.Services.Interfaces;
public interface IMovieService
{
    public Task<Movie> CreateMovieAsync(CreateMovieDTO createMovieDTO);
    Task<IReadOnlyList<MovieListDTO>> GetAllMovieAsync();
    Task<MovieDetailDTO> GetMovieDetail(Guid movieId);
    Task PostComment(string commentText, Guid movieId, Guid commenterId, string commenterName);
}
