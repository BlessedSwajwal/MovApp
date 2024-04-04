using Infrastructure.Common;
using Infrastructure.DTOs;
using OneOf;

namespace Infrastructure.Services.Interfaces;
public interface IUserAuthenticationService
{

    Task<OneOf<AuthResponse, CustomError>> LoginAsync(LoginModel model, string role);
    Task LogoutAsync();
    Task<Status> RegisterAsync(RegistrationModel model);
    //Task<Status> ChangePasswordAsync(ChangePasswordModel model, string username);

}
