using Infrastructure.Data;
using Infrastructure.DTOs;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovApp.Controllers;
[AllowAnonymous]
public class UserAuthenticationController(IUserAuthenticationService _authService) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Registration()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var result = await _authService.LoginAsync(model, UserRoles.user);
        if (result.StatusCode == 1)
        {
            return RedirectToAction(nameof(HomeController.Index));
        }
        else
        {
            TempData["msg"] = result.Message;
            return RedirectToAction(nameof(Login));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Registration(RegistrationModel model)
    {
        if (!ModelState.IsValid) { return View(model); }
        model.Role = UserRoles.user;
        model.UserType = UserRoles.user;
        var result = await _authService.RegisterAsync(model);
        TempData["msg"] = result.Message;
        return RedirectToAction(nameof(Registration));
    }
}
