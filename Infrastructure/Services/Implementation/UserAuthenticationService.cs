using Infrastructure.Authentication.Interfaces;
using Infrastructure.Common;
using Infrastructure.Data;
using Infrastructure.DTOs;
using Infrastructure.Persistence;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using OneOf;
using System.Net;
using System.Security.Claims;


namespace Infrastructure.Services.Implementation;
public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IJwtGenerator _jwtGenerator;

    public UserAuthenticationService(UserManager<ApplicationUser> userManager, IJwtGenerator jwtGenerator, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
    {
        this.userManager = userManager;
        _jwtGenerator = jwtGenerator;

        this.signInManager = signInManager;
        _context = context;
    }

    public async Task<OneOf<AuthResponse, CustomError>> LoginAsync(LoginModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username);

        if (user is null)
        {
            var error = new CustomError((int)HttpStatusCode.NotFound, "No such user found");
            return error;
        }
        var passwordCorrect = await userManager.CheckPasswordAsync(user!, model.Password);

        if (passwordCorrect)
        {
            Claim[] claims = [new Claim(ClaimTypes.Role, user.UserType)];
            await signInManager.SignInWithClaimsAsync(user, true, claims);
            //var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
            var token = _jwtGenerator.GenerateJwt(user);
            await Console.Out.WriteLineAsync(token);

            return new AuthResponse(token);
        }
        else
        {
            return new CustomError(404, "Invalid credentials");
        }

    }


    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }

    public async Task<ApplicationUser> GetAdmin()
    {
        var admin = await userManager.FindByEmailAsync("admin@admin.com");
        return admin;
    }

    public async Task<Status> RegisterAsync(RegistrationModel model)
    {

        var status = new Status();
        var userExists = await userManager.FindByNameAsync(model.Username);
        if (userExists != null)
        {
            status.StatusCode = 0;
            status.Message = "User already exist";
            return status;
        }
        ApplicationUser user = new ApplicationUser()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserType = model.UserType!,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
        };
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            status.StatusCode = 0;
            status.Message = "User creation failed";
            return status;
        }

        status.StatusCode = 1;
        status.Message = "You have registered successfully";
        return status;
    }
}
