using Infrastructure.DTOs.Movie;
using Infrastructure.Services.Email;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovApp.Models;
using System.Security.Claims;

namespace MovApp.Controllers;
public class MoviesController(IMovieService movieService, IEmailService emailService) : Controller
{
    public async Task<IActionResult> Index(bool NewMovieCreated = false, bool MovieDeleted = false, int page = 0)
    {
        ViewBag.NewMovieCreated = NewMovieCreated;
        ViewBag.MovieDeleted = MovieDeleted;
        ViewBag.Page = page;
        var movies = await movieService.GetMovies(page);
        return View(movies);
    }

    //[Authorize(Policy = "AdminRequirement")]
    //public async Task<IActionResult> MovieDashboard()
    //{
    //    var movies = await movieService.GetAllMovieAsync();
    //    return View(movies);
    //}

    [Authorize(Policy = "AdminRequirement")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Policy = "AdminRequirement")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await movieService.DeleteMovie(id);
        return RedirectToAction(nameof(Index), new { MovieDeleted = true });
    }

    [HttpPost]
    [Authorize(Policy = "AdminRequirement")]
    public async Task<ActionResult> Create(CreateMovieDTO model, IFormFile imageFile)
    {

        ModelState.Remove(nameof(model.ImageData));
        if (ModelState.IsValid && imageFile != null && imageFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                model.ImageData = memoryStream.ToArray();
            }

            //TODO: Create the movie
            await movieService.CreateMovieAsync(model);

            return RedirectToAction(nameof(Index), new { NewMovieCreated = true });
        }
        else
        {
            TempData["msg"] = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }

        // If the model is invalid or no image is uploaded, return to the create form
        return View(model);
    }

    [Authorize(Policy = "AdminRequirement")]
    public async Task<IActionResult> Update(Guid id)
    {
        var movie = await movieService.GetMovieDetail(id);
        return View(movie);
    }

    [HttpPost]
    [Authorize(Policy = "AdminRequirement")]
    public async Task<IActionResult> Update(MovieDetailDTO movie, IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                movie.Image = memoryStream.ToArray();
            }
        }
        if (ModelState.IsValid)
        {
            await movieService.Update(movie);
            return RedirectToAction(nameof(Detail), new { Id = movie.Id });
        }

        return View(movie);
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        var movieDTO = await movieService.GetMovieDetail(id);
        var hasRated = await movieService.HasUserAlreadyRated(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        ViewBag.HasRated = hasRated;

        var movie = new MovieDetailViewModel(User, movieDTO);
        return View(movie);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitComment()
    {

        string commentText = Request.Form["comment"]!;
        string movieId = Request.Form["movieId"]!;

        await movieService.PostComment(commentText, Guid.Parse(movieId), User.FindFirstValue(ClaimTypes.NameIdentifier)!, User.Identity!.Name!);


        return RedirectToAction(nameof(Detail), new { id = movieId });
    }

    public async Task<IActionResult> Trending([FromQuery] int page = 1)
    {
        ViewBag.currentPage = page;
        var movies = await movieService.GetTrendingMovies(page);
        return View(movies);
    }

    [HttpPost]
    public async Task<IActionResult> Rate(Guid movieId, string userId, int rating)
    {
        var movieDTO = await movieService.GetMovieDetail(movieId);
        await movieService.AddRating(movieDTO, userId, rating);
        return RedirectToAction(nameof(Detail), new { Id = movieId });
    }

    [HttpPost]
    public async Task<IActionResult> EmailShare(Guid movieId, string to)
    {
        var movie = await movieService.GetMovieDetail(movieId);
        await emailService.ShareMovie(to, movie);
        return RedirectToAction(nameof(Detail), new { Id = movieId });
    }

    [HttpPost]
    public async Task<IActionResult> AddTrending(string title, string description, string imageUrl, int? currentPage = 1)
    {
        var image_data = await movieService.FetchImageAsync(imageUrl);
        var createMovieDTO = new CreateMovieDTO() { Title = title, Description = description, ImageData = image_data };
        var movie = await movieService.CreateMovieAsync(createMovieDTO);
        TempData["msg"] = "Movie added succesfully!";
        return RedirectToAction(nameof(Trending), new { page = currentPage });
    }
}
