using Infrastructure.Authentication.Interfaces;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Authentication.Implementation;
public sealed class JwtGenerator : IJwtGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtGenerator(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Generates a JWT token for the user.
    /// </summary>
    /// <param name="User">The user for whom token is to be generated.</param>
    /// <returns></returns>
    public string GenerateJwt(ApplicationUser User)
    {
        SigningCredentials signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, User.Id),
            new Claim(ClaimTypes.Email, User.Email),
            new Claim(ClaimTypes.Name, User.FirstName + " " + User.LastName),
            new Claim("UserName", User.UserName),
            new Claim("UserType", User.UserType),
            new Claim(ClaimTypes.Role, User.UserType)
        };

        var jwt = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}

