using Infrastructure.DTOs.Movie;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovApp.Controllers;
[AllowAnonymous]
public class MoviesController(IMovieService movieService) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateMovieDTO model, IFormFile imageFile)
    {

        if (ModelState.IsValid && imageFile != null && imageFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                model.ImageData = memoryStream.ToArray();
            }

            //TODO: Create the movie
            await movieService.CreateMovieAsync(model);

            return RedirectToAction("Index", "Home"); // Redirect to home or another action
        }

        // If the model is invalid or no image is uploaded, return to the create form
        return View(model);
    }
}
