using Infrastructure.Common;
using Infrastructure.Data;
using Infrastructure.DTOs.Movie;
using OneOf;

namespace Infrastructure.Services.Interfaces;
public interface IMovieService
{
    //Task AddRating(MovieDetailDTO movieDto, string userId, int Rating);
    public Task<Movie> CreateMovieAsync(CreateMovieDTO createMovieDTO);
    Task DeleteMovie(Guid movieId);
    Task<byte[]> FetchImageAsync(string imageUrl);
    Task<IReadOnlyList<MovieListDTO>> GetMovies(int page);
    Task<OneOf<MovieDetailDTO, CustomError>> GetMovieDetail(Guid movieId);
    Task<List<TrendingMovieDTO>> GetTrendingMovies(int page);
    //Task<bool> HasUserAlreadyRated(Guid movieId, string userId);
    //Task PostComment(string commentText, Guid movieId, string commenterId, string commenterName);
    Task Update(MovieDetailDTO updatedMovie);
    Task<IReadOnlyList<MovieListDTO>> Search(string searchParam);
}
