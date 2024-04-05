using Infrastructure.Authentication;
using Infrastructure.Authentication.Implementation;
using Infrastructure.Authentication.Interfaces;
using Infrastructure.Authorization;
using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Implementation;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services.Implementation;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;
public static class DependencyRegister
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MovApp")));

        AddAuth(services, configuration);
        AddServicesAndRepo(services);

        services.AddIdentity<ApplicationUser, IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminRequirement", policy =>
            {
                policy.Requirements.Add(new AdminEmailRequirement());
            });
        });

        services.AddSingleton<IAuthorizationHandler, AdminEmailRequirementHandler>();

        AddAdmin.Add(configuration.GetValue<string>("AdminPassword")!);
        return services;
    }

    public static void AddAuth(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
        services.AddSingleton(Options.Create<JwtSettings>(jwtSettings));

        //Add Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
            .AddCookie(options =>
            {
                options.LoginPath = "/UserAuthentication/Login";

            })
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            });

        services.AddSingleton<IJwtGenerator, JwtGenerator>();
    }

    public static void AddServicesAndRepo(IServiceCollection services)
    {
        services.AddScoped<IMovieRepository, MovieRepository>();

        services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
        services.AddScoped<IMovieService, MovieService>();
    }


}
