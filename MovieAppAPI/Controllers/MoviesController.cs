using Infrastructure.DTOs.Movie;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public IActionResult Delete(Guid id)
    {
        movieService.DeleteMovie(id);
        return Ok();
    }
}
