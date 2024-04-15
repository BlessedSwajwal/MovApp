using Infrastructure;
using Infrastructure.DTOs.Movie;
using Infrastructure.Services.Email;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovApp.Models;
using System.Security.Claims;

namespace MovApp.Controllers;
public class MoviesController(IMovieService movieService, ICommentService commentService, IRatingService ratingService, IEmailService emailService) : Controller
{
    public async Task<IActionResult> Index(bool NewMovieCreated = false, bool MovieDeleted = false, int page = 0)
    {
        ViewBag.NewMovieCreated = NewMovieCreated;
        ViewBag.MovieDeleted = MovieDeleted;
        ViewBag.Page = page;
        var movies = await movieService.GetMovies(page);
        return View(movies);
    }


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
    public async Task<ActionResult> Create(CreateMovieDTO model)
    {

        if (ModelState.IsValid && model.ImageFile.Length > 0)
        {

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
        var movieResult = await movieService.GetMovieDetail(id);
        if (movieResult.IsT1) return RedirectToAction(nameof(Index));
        return View(movieResult.AsT0);
    }

    [HttpPost]
    [Authorize(Policy = "AdminRequirement")]
    public async Task<IActionResult> Update(UpdateMovieDetailsDTO updatedMovie)
    {

        if (ModelState.IsValid)
        {
            await movieService.Update(updatedMovie);

        }

        return RedirectToAction(nameof(Detail), new { Id = updatedMovie.MovieId });
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        var movieResult = await movieService.GetMovieDetail(id);

        if (movieResult.IsT1)
        {
            TempData["msg"] = movieResult.AsT1.Message;
            return RedirectToAction(nameof(Index));
        }

        var hasRated = await ratingService.HasUserAlreadyRated(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        ViewBag.HasRated = hasRated;

        var movie = new MovieDetailViewModel(User, movieResult.AsT0);
        return View(movie);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitComment()
    {

        string commentText = Request.Form["comment"]!;
        string movieId = Request.Form["movieId"]!;

        await commentService.PostComment(commentText, Guid.Parse(movieId), User.FindFirstValue(ClaimTypes.NameIdentifier)!, User.Identity!.Name!);


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
        if (rating < 0 || rating > 10) return Problem(statusCode: 400, detail: "Rating must be between 0 and 10");
        var movieResult = await movieService.GetMovieDetail(movieId);

        if (movieResult.IsT1) return BadRequest();

        await ratingService.AddRating(movieResult.AsT0, userId, rating);
        return RedirectToAction(nameof(Detail), new { Id = movieId });
    }

    [HttpPost]
    public async Task<IActionResult> EmailShare(Guid movieId, string to)
    {
        var movieResult = await movieService.GetMovieDetail(movieId);

        if (movieResult.IsT1)
        {
            return BadRequest();
        }

        emailService.ShareMovie(to, movieResult.AsT0);
        return RedirectToAction(nameof(Detail), new { Id = movieId });
    }

    //[HttpPost]
    //public async Task<IActionResult> AddTrending(string title, string description, DateOnly releaseDate, string imageUrl, int? currentPage = 1)
    //{
    //    var image_data = await movieService.FetchImageAsync(imageUrl);
    //    var createMovieDTO = new CreateMovieDTO(title, description, releaseDate, image_data);
    //    var movie = await movieService.CreateMovieAsync(createMovieDTO);
    //    TempData["msg"] = "Movie added succesfully!";
    //    return RedirectToAction(nameof(Trending), new { page = currentPage });
    //}

    [HttpGet]
    public async Task<IActionResult> Search(string searchText)
    {
        var movies = await movieService.Search(searchText);
        return View(movies);
    }

    [HttpPost]
    public IActionResult Search()
    {
        var searchText = Request.Form["searchText"];
        if (string.IsNullOrEmpty(searchText))
        {
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Search), new { searchText });
    }

    [HttpGet]
    [Route("/notifications")]
    public IActionResult Notification()
    {
        var notificationMovies = Notifications.NotificationsMovies;
        return View(notificationMovies);
    }

    public IActionResult UpdateImage([FromQuery] Guid movieId)
    {
        ViewBag.MovieId = movieId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateImage([FromForm] UpdateImageDTO updateImageDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await movieService.UpdateImage(updateImageDTO);
        return RedirectToAction(nameof(Detail), new { id = updateImageDTO.movieId });
    }
}
