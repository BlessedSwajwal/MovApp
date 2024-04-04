using Infrastructure.DTOs;

namespace Infrastructure.Services.Interfaces;
public interface IUserAuthenticationService
{

    Task<Status> LoginAsync(LoginModel model, string role);
    Task LogoutAsync();
    Task<Status> RegisterAsync(RegistrationModel model);
    //Task<Status> ChangePasswordAsync(ChangePasswordModel model, string username);

}
