using Infrastructure.DTOs.Movie;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MovieAppAPI.Controllers;
[Route("Movies")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MoviesController(IMovieService movieService) : ControllerBase
{
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

        //if (imageFile != null && imageFile.Length > 0)
        //{
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        await imageFile.CopyToAsync(memoryStream);
        //        movie.Image = memoryStream.ToArray();
        //    }
        //}

        await movieService.Update(updatedMovie);

        return Ok("Updated");
    }
}

public record UpdateMovie(Guid movieId, string name, string description);
