using Infrastructure.Data;
using Infrastructure.DTOs;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovAppAPI.Controllers;
[Route("Account")]
[ApiController]
[AllowAnonymous]
public class UserAuthenticationController(IUserAuthenticationService _authService) : ControllerBase
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        model.Role = UserRoles.user;
        model.UserType = UserRoles.user;
        var result = await _authService.RegisterAsync(model);
        return Ok(result);
    }
}
