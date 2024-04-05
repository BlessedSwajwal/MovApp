using Infrastructure.Data;
using Infrastructure.DTOs.Movie;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services.Interfaces;
using Mapster;

namespace Infrastructure.Services.Implementation;
public class MovieService(IMovieRepository movieRepository) : IMovieService
{
    public async Task<Movie> CreateMovieAsync(CreateMovieDTO createMovieDTO)
    {
        var movie = new Movie(createMovieDTO.Title, createMovieDTO.Description, createMovieDTO.ImageData);

        movieRepository.Create(movie);
        await movieRepository.SaveAsync();
        return movie;
    }

    public async Task<IReadOnlyList<MovieListDTO>> GetAllMovieAsync()
    {
        var movies = await movieRepository.GetAllAsync();
        var movieDTOs = movies.Adapt<List<MovieListDTO>>();
        return movieDTOs.AsReadOnly();
    }

    public async Task<MovieDetailDTO> GetMovieDetail(Guid movieId)
    {
        var movie = movieRepository.GetMovieDetail(movieId);
        var comments = await movieRepository.GetCommentsForAMovie(movieId);
        var result = movie.BuildAdapter().AddParameters("Comments", comments).AdaptToType<MovieDetailDTO>();
        return result;
    }

    public async Task PostComment(string commentText, Guid movieId, Guid commenterId, string commenterName)
    {
        var comment = new Comment(Guid.NewGuid(), commentText, movieId, commenterId, commenterName);
        movieRepository.AddComment(comment);
        //await movieRepository.SaveAsync();
    }
}
