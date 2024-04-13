using Infrastructure.Common;
using Infrastructure.Data;
using Infrastructure.DTOs;
using OneOf;

namespace Infrastructure.Services.Interfaces;
public interface IUserAuthenticationService
{
    Task<ApplicationUser> GetAdmin();
    Task<OneOf<AuthResponse, CustomError>> LoginAsync(LoginModel model);
    Task LogoutAsync();
    Task<Status> RegisterAsync(RegistrationModel model);

}
