﻿using Infrastructure.Authentication;
using Infrastructure.Authentication.Implementation;
using Infrastructure.Authentication.Interfaces;
using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Implementation;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services.Email;
using Infrastructure.Services.EmailService;
using Infrastructure.Services.Implementation;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MovApp")));


        AddAuth(services, configuration);
        AddServicesAndRepo(services, configuration);

        services.AddIdentity<ApplicationUser, IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>();

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

        services.AddScoped<IMovieRepository, MovieRepository>();

        services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
        //services.AddScoped<IMovieService, MovieService>();
        var emailSettings = new EmailSettings();
        configuration.GetSection(EmailSettings.SectionName).Bind(emailSettings);
        services.AddSingleton(Options.Create<EmailSettings>(emailSettings));
    }


}
