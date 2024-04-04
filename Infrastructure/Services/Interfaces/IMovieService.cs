using Infrastructure.Data;
using Infrastructure.DTOs.Movie;

namespace Infrastructure.Services.Interfaces;
public interface IMovieService
{
    public Task<Movie> CreateMovieAsync(CreateMovieDTO createMovieDTO);
}
