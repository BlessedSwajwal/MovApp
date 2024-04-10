using Infrastructure.DTOs.Movie;
using Infrastructure.Services.Email;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MovAppAPI.Controllers;

[Route("Movies")]
[ApiController]
public class MoviesController(IMovieService movieService, IEmailService emailService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> All(int page = 0)
    {
        var movies = await movieService.GetMovies(page);
        return Ok(movies);
    }

    [HttpPost("Create")]
    [Authorize(Policy = "AdminRequirement")]
    public async Task<ActionResult> Create(CreateMovieDTO model, IFormFile imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                model.ImageData = memoryStream.ToArray();
            }

            //TODO: Create the movie
            var movie = await movieService.CreateMovieAsync(model);

            return Ok(movie);
        }
        else
        {
            return Problem(title: "Error", detail: "Image error", statusCode: 401);
        }

    }

    [Authorize(Policy = "AdminRequirement")]
    [HttpGet("Delete")]
    public IActionResult Delete([FromQuery] Guid id)
    {
        movieService.DeleteMovie(id);
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
    [Authorize(Policy = "AdminRequirement")]
    public async Task<IActionResult> Update(MovieDetailDTO updatedMovie)
    {
        await movieService.Update(updatedMovie);
        return Ok("Updated");
    }

    [HttpPost("Comment")]
    public async Task<IActionResult> SubmitComment(Guid movieId, string commentText)
    {

        await movieService.PostComment(commentText, movieId, User.FindFirstValue(ClaimTypes.NameIdentifier)!, User.Identity!.Name!);


        return Ok(new { movieId, commentText });
    }

    [HttpGet("Trending")]
    public async Task<IActionResult> Trending([FromQuery] int page = 1)
    {
        var movies = await movieService.GetTrendingMovies(page);
        return Ok(movies);
    }

    [HttpPost("Rate")]
    public async Task<IActionResult> Rate(Guid movieId, int rating)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var movieDTO = await movieService.GetMovieDetail(movieId);
        await movieService.AddRating(movieDTO, userId, rating);
        return Ok(new { movieDTO, rated = rating });
    }

    [HttpPost("Share")]
    public async Task<IActionResult> EmailShare(Guid movieId, string to)
    {
        var movie = await movieService.GetMovieDetail(movieId);
        await emailService.ShareMovie(to, movie);
        return Ok(new
        {
            status = 200,
            detail = $"Movie has been shared to {to}"
        });
    }

    [HttpPost("Trending")]
    public async Task<IActionResult> AddTrending(string title, string description, string imageUrl, int? currentPage = 1)
    {
        var image_data = await movieService.FetchImageAsync(imageUrl);
        var createMovieDTO = new CreateMovieDTO() { Title = title, Description = description, ImageData = image_data };
        var movie = await movieService.CreateMovieAsync(createMovieDTO);
        return Ok(movie);
    }
}

public record UpdateMovie(Guid movieId, string name, string description);
