using Infrastructure.Data;

namespace Infrastructure.Authentication.Interfaces;

public interface IJwtGenerator
{
    string GenerateJwt(ApplicationUser User, string role);
}
