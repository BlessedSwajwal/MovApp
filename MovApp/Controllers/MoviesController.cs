﻿using Infrastructure.DTOs.Movie;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovApp.Models;
using System.Security.Claims;

namespace MovApp.Controllers;
public class MoviesController(IMovieService movieService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var movies = await movieService.GetAllMovieAsync();
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
    public IActionResult Delete(Guid id)
    {
        movieService.DeleteMovie(id);
        return RedirectToAction(nameof(Index));
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

            return RedirectToAction(nameof(Index));
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

    public async Task<IActionResult> Detail(Guid id)
    {
        var movie = new MovieDetailViewModel(User, await movieService.GetMovieDetail(id));
        return View(movie);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitComment()
    {

        string commentText = Request.Form["comment"]!;
        string movieId = Request.Form["movieId"]!;


        await movieService.PostComment(commentText, Guid.Parse(movieId), Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value), User.Identity!.Name!);


        return RedirectToAction(nameof(Detail), new { id = movieId });
    }
}
