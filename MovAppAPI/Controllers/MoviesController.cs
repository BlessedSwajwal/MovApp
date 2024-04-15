using Infrastructure.DTOs.Movie;
using Infrastructure.Services.Email;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovAppAPI.RequestContract;
using System.Security.Claims;

namespace MovAppAPI.Controllers;

[Route("Movies")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MoviesController(IMovieService movieService, ICommentService commentService, IRatingService ratingService, IEmailService emailService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> All(int page = 0)
    {
        var movies = await movieService.GetMovies(page);
        return Ok(movies);
    }

    [HttpPost("Create")]
    [Authorize("AdminRequirement")]
    public async Task<ActionResult> Create([FromForm] CreateMovieDTO createMovieRequest)
    {
        if (!ModelState.IsValid || createMovieRequest.ImageFile == null || createMovieRequest.ImageFile.Length <= 0)
        {
            return BadRequest(ModelState);
        }

        //TODO: Create the movie
        var movie = await movieService.CreateMovieAsync(createMovieRequest);

        return Ok(movie);
    }


    [HttpDelete("Delete")]
    [Authorize("AdminRequirement")]
    public async Task<IActionResult> Delete([FromQuery] Guid id)
    {
        await movieService.DeleteMovie(id);
        return Ok("Deleted");
    }

    [HttpGet("Detail")]
    public async Task<IActionResult> Detail([FromQuery] Guid id)
    {
        var movieResult = await movieService.GetMovieDetail(id);
        if (movieResult.IsT1)
        {
            var error = movieResult.AsT1;
            return Problem(statusCode: error.StatusCode, detail: error.Message);
        }

        var movieDetail = movieResult.AsT0;

        var hasRated = await ratingService.HasUserAlreadyRated(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var movie = new { hasRated = hasRated, movie = movieDetail };
        return Ok(movie);
    }

    [HttpPut("Update")]
    [Authorize("AdminRequirement")]
    public async Task<IActionResult> Update([FromForm] UpdateMovieDetailsDTO updatedMovie)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await movieService.Update(updatedMovie);
        return Ok("Updated");
    }

    [HttpPost("Comment")]
    public async Task<IActionResult> SubmitComment([FromBody] CommentModel commentModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await commentService.PostComment(commentModel.commentText, commentModel.movieId, User.FindFirstValue(ClaimTypes.NameIdentifier)!, User.Identity!.Name!);

        return Ok(new { commentModel.commentText, commentModel.movieId });
    }

    [HttpGet("Trending")]
    [Authorize("AdminRequirement")]
    public async Task<IActionResult> Trending([FromQuery] int page = 1)
    {
        var movies = await movieService.GetTrendingMovies(page);
        return Ok(movies);
    }

    [HttpGet("Rate")]
    public async Task<IActionResult> Rate([FromQuery] Guid movieId, [FromQuery] int rating)
    {
        if (rating < 0 || rating > 10) return Problem(statusCode: 400, detail: "Rating must be between 0 and 10");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var movieResult = await movieService.GetMovieDetail(movieId);

        if (movieResult.IsT1) return RedirectToAction(nameof(Index));

        await ratingService.AddRating(movieResult.AsT0, userId, rating);
        return Ok(new { movieResult, rated = rating });
    }

    [HttpPost("Share")]
    public async Task<IActionResult> EmailShare(EmailShareRequest emailShareRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var movieResult = await movieService.GetMovieDetail(emailShareRequest.movieId);

        if (movieResult.IsT1) return BadRequest(movieResult);

        await emailService.ShareMovie(emailShareRequest.to, movieResult.AsT0);
        return Ok(new
        {
            status = 200,
            detail = $"Movie has been shared to {emailShareRequest.to}"
        });
    }

    [HttpPut("UpdateImage")]
    public async Task<IActionResult> UpdateImage([FromForm] UpdateImageDTO updateImageDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await movieService.UpdateImage(updateImageDTO);
        return Ok();
    }

    [HttpGet("Search")]
    public async Task<IActionResult> SearchByName([FromQuery] string searchQuery)
    {
        var movies = await movieService.Search(searchQuery);
        return Ok(movies);
    }
}

