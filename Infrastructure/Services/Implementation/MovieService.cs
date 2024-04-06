using Infrastructure.Data;
using Infrastructure.DTOs.Movie;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services.Interfaces;
using Mapster;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.Services.Implementation;
public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly HttpClient _httpClient;
    private readonly TmdbSettings _settings;

    public MovieService(IMovieRepository movieRepository, HttpClient httpClient, IOptions<TmdbSettings> settings)
    {
        _movieRepository = movieRepository;
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<Movie> CreateMovieAsync(CreateMovieDTO createMovieDTO)
    {
        var movie = new Movie(createMovieDTO.Title, createMovieDTO.Description, createMovieDTO.ImageData);

        _movieRepository.Create(movie);
        await _movieRepository.SaveAsync();
        return movie;
    }

    public async Task<IReadOnlyList<MovieListDTO>> GetAllMovieAsync()
    {
        var movies = await _movieRepository.GetAllAsync();
        var movieDTOs = movies.Adapt<List<MovieListDTO>>();
        return movieDTOs.AsReadOnly();
    }

    public async Task<MovieDetailDTO> GetMovieDetail(Guid movieId)
    {
        var movie = _movieRepository.GetMovieDetail(movieId);
        var comments = await _movieRepository.GetCommentsForAMovie(movieId);
        var result = movie.BuildAdapter().AddParameters("Comments", comments).AdaptToType<MovieDetailDTO>();
        return result;
    }

    public async Task PostComment(string commentText, Guid movieId, Guid commenterId, string commenterName)
    {
        var comment = new Comment(Guid.NewGuid(), commentText, movieId, commenterId, commenterName);
        _movieRepository.AddComment(comment);
        //await movieRepository.SaveAsync();
    }

    public void DeleteMovie(Guid movieId)
    {
        _movieRepository.DeleteMovie(movieId);
    }

    public async Task<List<CreateMovieDTO>> GetTrendingMovies(int page = 1)
    {
        var movies = new List<CreateMovieDTO>();
        var response = await _httpClient.GetAsync($"/3/discover/movie?include_adult=false&include_video=false&language=en-US&page={page}&sort_by=popularity.desc&api_key={_settings.TmdbAPIKey}");

        if (!response.IsSuccessStatusCode)
        {
            //TODO: Handle error properly
            await Console.Out.WriteLineAsync("Error while fetching movies from TMDB");
        }

        var root = JsonDocument.Parse(response.Content.ReadAsStream()).RootElement;
        var movieList = root.GetProperty("results");

        foreach (var movie in movieList.EnumerateArray())
        {
            var movieRoot = JsonDocument.Parse(movie.ToString()).RootElement;
            var title = movieRoot.GetProperty("original_title").GetString();
            var description = movieRoot.GetProperty("overview").GetString();
            var image_url = movieRoot.GetProperty("backdrop_path").GetString();
            byte[] imageBytes = await _httpClient.GetByteArrayAsync("https://image.tmdb.org/t/p/original" + image_url);
            //TODO: Error handling

            movies.Add(new CreateMovieDTO
            {
                Title = title,
                Description = description,
                ImageData = imageBytes
            });

            Console.WriteLine(movie);
        }

        return movies;
    }

}
