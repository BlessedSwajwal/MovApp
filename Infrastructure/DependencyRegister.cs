using Infrastructure.Authentication;
using Infrastructure.Authentication.Implementation;
using Infrastructure.Authentication.Interfaces;
using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Implementation;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Repositories.SQLImplementation;
using Infrastructure.Services.Email;
using Infrastructure.Services.EmailService;
using Infrastructure.Services.Implementation;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Infrastructure;
public static class DependencyRegister
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var conn_string = configuration.GetConnectionString("MovApp");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conn_string));


        AddServicesAndRepo(services, configuration);

        services.AddIdentity<ApplicationUser, IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>();

        AddAuth(services, configuration);

        //AddAdmin.Add(configuration.GetValue<string>("AdminPassword")!);

        //Movie settings
        var tmdbSettings = new TmdbSettings();
        configuration.GetSection(TmdbSettings.SectionName).Bind(tmdbSettings);
        services.AddSingleton(Options.Create<TmdbSettings>(tmdbSettings));

        return services;
    }

    public static void AddAuth(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
        services.AddSingleton(Options.Create<JwtSettings>(jwtSettings));

        if (configuration.GetValue<string>("ProjType")!.Equals("MVC"))
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {

            });
        }
        else
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

        }

        services.AddSingleton<IJwtGenerator, JwtGenerator>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminRequirement", policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, UserRoles.admin);
            });
        });
    }

    public static void AddServicesAndRepo(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEmailService, EmailService>();

        services.AddHttpClient<IMovieService, MovieService>(op =>
        {
            op.BaseAddress = new Uri("https://api.themoviedb.org");
        });

        if (configuration.GetValue<bool>("UseSql"))
        {
            services.AddScoped<IMovieRepository, SQLMovieRepository>();
        }
        else
        {
            services.AddScoped<IMovieRepository, MovieRepository>();
        }


        services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
        //services.AddScoped<IMovieService, MovieService>();
        var emailSettings = new EmailSettings();
        configuration.GetSection(EmailSettings.SectionName).Bind(emailSettings);
        services.AddSingleton(Options.Create<EmailSettings>(emailSettings));
    }


}
