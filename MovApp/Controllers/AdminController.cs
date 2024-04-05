using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MovApp.Controllers;
public class AdminController(IUserAuthenticationService userAuthenticationService) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    //[AllowAnonymous]
    //public IActionResult Login()
    //{
    //    return View();
    //}

    //[AllowAnonymous]
    //[HttpPost]
    //public async Task<IActionResult> Login(LoginModel model)
    //{
    //    if (!ModelState.IsValid)
    //        return View(model);

    //    var result = await userAuthenticationService.LoginAsync(model);

    //    return result.Match<IActionResult>(
    //            authResponse => RedirectToAction("Index", "Home"),
    //            errorResult =>
    //            {
    //                TempData["msg"] = "Error while logging in! Check admin credentials and try again!.";
    //                return RedirectToAction(nameof(Login));
    //            }
    //        );

    //    //var admin = await userAuthenticationService.GetAdmin();
    //    //var hashedPass = admin.PasswordHash;

    //    //var claims = new List<Claim>
    //    //{
    //    //    new Claim(ClaimTypes.Role, UserRoles.admin)
    //    //};

    //    //var result = await userManager.CheckPasswordAsync(admin, model.Password);


    //    //if (result)
    //    //{
    //    //    await signInManager.SignInWithClaimsAsync(admin, false, claims);
    //    //    return RedirectToAction("Index", "Home");
    //    //}
    //    //else
    //    //{
    //    //    TempData["msg"] = "Error while logging in! Check admin credentials and try again!.";
    //    //    return RedirectToAction(nameof(Login));
    //    //}

    //}
}
