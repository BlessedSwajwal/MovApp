using Infrastructure.Data;
using Infrastructure.DTOs;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MovApp.Controllers;
public class AdminController(IUserAuthenticationService userAuthenticationService, SignInManager<ApplicationUser> signInManager) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {

        if (!ModelState.IsValid)
            return View(model);

        var admin = await userAuthenticationService.GetAdmin();
        var hashedPass = admin.PasswordHash;

        var result = await signInManager.PasswordSignInAsync(admin, model.Password, false, true);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            TempData["msg"] = "Error while logging in! Check admin credentials and try again!.";
            return RedirectToAction(nameof(Login));
        }

    }
}
