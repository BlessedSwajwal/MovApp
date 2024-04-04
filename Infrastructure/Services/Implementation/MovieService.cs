using Infrastructure.Data;
using Infrastructure.DTOs.Movie;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services.Implementation;
public class MovieService(IMovieRepository movieRepository) : IMovieService
{
    public Task<Movie> CreateMovieAsync(CreateMovieDTO createMovieDTO)
    {
        var movie = new Movie(createMovieDTO.Title, createMovieDTO.Description, createMovieDTO.ImageData);

        movieRepository.Create(movie);
        return Task.FromResult(movie);
    }
}
