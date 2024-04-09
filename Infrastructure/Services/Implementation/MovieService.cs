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
        var movie = Movie.CreateNew(createMovieDTO.Title, createMovieDTO.Description, createMovieDTO.ImageData);

        await _movieRepository.Create(movie);
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
        var movie = await _movieRepository.GetMovieDetail(movieId);
        var comments = await _movieRepository.GetCommentsForAMovie(movieId);
        var result = movie.BuildAdapter().AddParameters("Comments", comments).AdaptToType<MovieDetailDTO>();
        return result;
    }

    public async Task PostComment(string commentText, Guid movieId, Guid commenterId, string commenterName)
    {
        var comment = new Comment(Guid.NewGuid(), commentText, movieId, commenterId, commenterName);
        await _movieRepository.AddComment(comment);
        //await movieRepository.SaveAsync();
    }

    public void DeleteMovie(Guid movieId)
    {
        _movieRepository.DeleteMovie(movieId);
    }

    public async Task<List<TrendingMovieDTO>> GetTrendingMovies(int page = 1)
    {
        var movies = new List<TrendingMovieDTO>();
        var response = await _httpClient.GetAsync($"/3/discover/movie?include_adult=true&include_video=false&language=en-US&page={page}&sort_by=popularity.desc&api_key={_settings.TmdbAPIKey}");

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
            var title = movieRoot.GetProperty("original_title").GetString()!;
            var description = movieRoot.GetProperty("overview").GetString()!;
            var image_url = "https://image.tmdb.org/t/p/original" + movieRoot.GetProperty("backdrop_path").GetString()!;
            //byte[] imageBytes = await _httpClient.GetByteArrayAsync("https://image.tmdb.org/t/p/original" + image_url);
            //TODO: Error handling

            movies.Add(new TrendingMovieDTO(title, description, image_url));

            Console.WriteLine(movie);
        }

        return movies;
    }

    public async Task Update(MovieDetailDTO updatedMovie)
    {
        var commentIds = updatedMovie.Comments.Select(c => c.Id).ToList();
        var movie = Movie.Create(updatedMovie.Id, updatedMovie.Name, updatedMovie.Description, updatedMovie.Image, updatedMovie.Rating, updatedMovie.TotalRates, commentIds);
        await _movieRepository.Update(movie);
    }


    public async Task AddRating(MovieDetailDTO movieDto, string userId, int Rating)
    {
        var movie = Movie.Create(movieDto.Id, movieDto.Name, movieDto.Description, movieDto.Image, movieDto.Rating, movieDto.TotalRates, new List<Guid>());
        await _movieRepository.AddRating(movie, userId, Rating);
    }

    public async Task<bool> HasUserAlreadyRated(Guid movieId, string userId)
    {
        var hasRated = await _movieRepository.HasUserRatedMovie(movieId, userId);
        return hasRated;
    }

    public async Task<byte[]> FetchImageAsync(string imageUrl)
    {
        byte[] imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
        //TODO: Error handling in case http request is not successful
        return imageBytes;
    }
}
