using Infrastructure.Authentication.Interfaces;
using Infrastructure.Data;
using Infrastructure.DTOs;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Implementation;
public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IJwtGenerator _jwtGenerator;
    //private readonly SignInManager<ApplicationUser> signInManager;

    public UserAuthenticationService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager /*, SignInManager<ApplicationUser> signInManager*/, IJwtGenerator jwtGenerator)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        _jwtGenerator = jwtGenerator;
        //this.signInManager = signInManager;
    }

    public async Task<Status> LoginAsync(LoginModel model, string role)
    {
        var status = new Status();
        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            status.StatusCode = 0;
            status.Message = "Invalid username";
            return status;
        }

        //if (!await userManager.CheckPasswordAsync(user, model.Password))
        //{
        //    status.StatusCode = 0;
        //    status.Message = "Invalid Password";
        //    return status;
        //}
        var passwordCorrect = await userManager.CheckPasswordAsync(user, model.Password);


        //var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
        if (passwordCorrect)
        {
            var token = _jwtGenerator.GenerateJwt(user, role);
            await Console.Out.WriteLineAsync(token);
            //var userRoles = await userManager.GetRolesAsync(user);

            //var authClaims = new List<Claim>
            //    {
            //        new Claim(ClaimTypes.Name, user.UserName),
            //    };

            //foreach (var userRole in userRoles)
            //{
            //    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            //}

            status.StatusCode = 1;
            status.Message = "Logged in successfully";
        }

        else
        {
            status.StatusCode = 0;
            status.Message = "Error on logging in";
        }

        return status;
    }


    public Task LogoutAsync()
    {
        throw new NotImplementedException();
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

        if (!await roleManager.RoleExistsAsync(model.Role))
            await roleManager.CreateAsync(new IdentityRole(model.Role));


        if (await roleManager.RoleExistsAsync(model.Role))
        {
            await userManager.AddToRoleAsync(user, model.Role);
        }

        status.StatusCode = 1;
        status.Message = "You have registered successfully";
        return status;
    }
}
