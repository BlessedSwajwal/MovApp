using Infrastructure.Data;
using Infrastructure.DTOs;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovieAppAPI.Controllers;


[Route("Account")]
[ApiController]
[AllowAnonymous]
public class UserAuthenticationController(IUserAuthenticationService _authService) : ControllerBase
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var result = await _authService.LoginAsync(model);

        return result.Match<IActionResult>(
                authResponse => Ok(authResponse),

                errorResponse =>
                {
                    return Problem(detail: errorResponse.Message, statusCode: errorResponse.StatusCode);
                }
            );
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Registration(RegistrationModel model)
    {
        model.Role = UserRoles.user;
        model.UserType = UserRoles.user;
        var result = await _authService.RegisterAsync(model);
        return Ok(result);
    }
}
