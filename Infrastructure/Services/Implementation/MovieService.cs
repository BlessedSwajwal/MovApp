using Infrastructure.Common;
using Infrastructure.Data;
using Infrastructure.DTOs.Movie;
using Infrastructure.Persistence.Repositories.Interfaces;
using Infrastructure.Services.Interfaces;
using Mapster;
using Microsoft.Extensions.Options;
using OneOf;
using System.Net;
using System.Text.Json;

namespace Infrastructure.Services.Implementation;
public partial class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly HttpClient _httpClient;
    private readonly TmdbSettings _settings;

    public MovieService(IMovieRepository movieRepository, HttpClient httpClient, IOptions<TmdbSettings> settings, ICommentRepository commentRepository)
    {
        _movieRepository = movieRepository;
        _httpClient = httpClient;
        _settings = settings.Value;
        _commentRepository = commentRepository;
    }

    public async Task<Movie> CreateMovieAsync(CreateMovieDTO createMovieDTO)
    {
        var uniqueFileName = await SharedFile.SaveFile(createMovieDTO.ImageFile);

        var movie = Movie.CreateNew(createMovieDTO.Title, createMovieDTO.Description, uniqueFileName, createMovieDTO.ReleaseDate);

        await _movieRepository.Create(movie);
        return movie;
    }

    public async Task<IReadOnlyList<MovieListDTO>> GetMovies(int page)
    {
        var movies = await _movieRepository.GetMovies(page);
        var movieDTOs = movies.Adapt<List<MovieListDTO>>();
        return movieDTOs.AsReadOnly();
    }

    public async Task<OneOf<MovieDetailDTO, CustomError>> GetMovieDetail(Guid movieId)
    {
        var movie = await _movieRepository.GetMovieDetail(movieId);

        if (movie is null)
        {
            return new CustomError((int)HttpStatusCode.NotFound, $"The movie with id: {movieId} does not exist");
        }

        var comments = await _commentRepository.GetCommentsForAMovie(movieId);
        var result = movie.BuildAdapter().AddParameters("Comments", comments).AdaptToType<MovieDetailDTO>();
        return result;
    }

    public async Task DeleteMovie(Guid movieId)
    {
        await _movieRepository.DeleteMovie(movieId);
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

    public async Task Update(UpdateMovieDetailsDTO updatedMovie)
    {
        await _movieRepository.UpdateNameAndDesc(updatedMovie.MovieId, updatedMovie.name, updatedMovie.description, updatedMovie.releaseDate);
    }

    public async Task<byte[]> FetchImageAsync(string imageUrl)
    {
        byte[] imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
        //TODO: Error handling in case http request is not successful
        return imageBytes;
    }

    public async Task<IReadOnlyList<MovieListDTO>> Search(string searchParam)
    {
        var movies = await _movieRepository.Search(searchParam);
        var movieDTOs = movies.Adapt<List<MovieListDTO>>();
        return movieDTOs.AsReadOnly();
    }

    public async Task UpdateImage(UpdateImageDTO updateImageDTO)
    {
        var fileName = await SharedFile.SaveFile(updateImageDTO.ImageFile);
        await _movieRepository.UpdateImage(updateImageDTO.movieId, fileName);
    }
}

