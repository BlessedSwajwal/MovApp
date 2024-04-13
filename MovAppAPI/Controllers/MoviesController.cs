using Infrastructure.Data;
using Infrastructure.DTOs.Movie;
using Infrastructure.Services.Email;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MovAppAPI.Controllers;

[Route("Movies")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MoviesController(IMovieService movieService, IEmailService emailService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> All(int page = 0)
    {
        ClaimsPrincipal user = User;
        var movies = await movieService.GetMovies(page);
        return Ok(movies);
    }

    [HttpPost("Create")]
    public async Task<ActionResult> Create([FromForm] CreateMovieFromAPI createMovie)
    {
        byte[] image_data;
        if (createMovie.imageFile != null && createMovie.imageFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await createMovie.imageFile.CopyToAsync(memoryStream);
                image_data = memoryStream.ToArray();
            }

            var createMovieDTO = new CreateMovieDTO(createMovie.title, createMovie.description, createMovie.ReleaseDate, image_data);
            //TODO: Create the movie
            var movie = await movieService.CreateMovieAsync(createMovieDTO);

            return Ok(movie);
        }
        else
        {
            return Problem(title: "Error", detail: "Image error", statusCode: 401);
        }

    }


    [HttpGet("Delete")]
    public async Task<IActionResult> Delete([FromQuery] Guid id)
    {
        await movieService.DeleteMovie(id);
        return Ok("Deleted");
    }

    [HttpGet("Detail")]
    public async Task<IActionResult> Detail([FromQuery] Guid id)
    {
        var movieDTO = await movieService.GetMovieDetail(id);
        var hasRated = await movieService.HasUserAlreadyRated(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var movie = new { hasRated = hasRated, movie = movieDTO };
        return Ok(movie);
    }

    [HttpPost("Update")]
    public async Task<IActionResult> Update(UpdateNameAndDesc updatedMovie)
    {
        var movieDetail = new MovieDetailDTO()
        {
            Id = updatedMovie.movieId,
            Name = updatedMovie.title,
            Description = updatedMovie.description,
            Image = updatedMovie.image_data,
            Rating = updatedMovie.rating,
            TotalRates = updatedMovie.totalRates,
            Comments = updatedMovie.comments,
        };
        await movieService.Update(movieDetail);
        return Ok("Updated");
    }

    [HttpPost("Comment")]
    public async Task<IActionResult> SubmitComment([FromBody] CommentModel commentModel)
    {
        ClaimsPrincipal user = User;
        await movieService.PostComment(commentModel.commentText, commentModel.movieId, User.FindFirstValue(ClaimTypes.NameIdentifier)!, User.Identity!.Name!);

        return Ok(new { commentModel.commentText, commentModel.movieId });
    }

    [HttpGet("Trending")]
    public async Task<IActionResult> Trending([FromQuery] int page = 1)
    {
        var movies = await movieService.GetTrendingMovies(page);
        return Ok(movies);
    }

    [HttpGet("Rate")]
    public async Task<IActionResult> Rate([FromQuery] Guid movieId, [FromQuery] int rating)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var movieDTO = await movieService.GetMovieDetail(movieId);
        await movieService.AddRating(movieDTO, userId, rating);
        return Ok(new { movieDTO, rated = rating });
    }

    [HttpPost("Share")]
    public async Task<IActionResult> EmailShare(EmailShareRequest emailShareRequest)
    {
        var movie = await movieService.GetMovieDetail(emailShareRequest.movieId);
        await emailService.ShareMovie(emailShareRequest.to, movie);
        return Ok(new
        {
            status = 200,
            detail = $"Movie has been shared to {emailShareRequest.to}"
        });
    }

    [HttpPost("Trending")]
    public async Task<IActionResult> AddTrending(string title, string description, DateOnly releaseDate, string imageUrl, int? currentPage = 1)
    {
        var image_data = await movieService.FetchImageAsync(imageUrl);
        var createMovieDTO = new CreateMovieDTO(title, description, releaseDate, image_data);
        var movie = await movieService.CreateMovieAsync(createMovieDTO);
        return Ok(movie);
    }

    [HttpGet("Search")]
    public async Task<IActionResult> SearchByName([FromQuery] string searchQuery)
    {
        var movies = await movieService.Search(searchQuery);
        return Ok(movies);
    }
}

public record UpdateMovie(Guid movieId, string name, string description);
public record CommentModel(Guid movieId, string commentText);
public record UpdateNameAndDesc(Guid movieId, string title, string description, List<Comment> comments, int rating, int totalRates, byte[] image_data);
public record EmailShareRequest(Guid movieId, string to);

public record CreateMovieFromAPI(string title, string description, DateOnly ReleaseDate, IFormFile imageFile);