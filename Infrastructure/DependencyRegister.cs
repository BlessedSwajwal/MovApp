using Infrastructure.Authentication;
using Infrastructure.Authentication.Implementation;
using Infrastructure.Authentication.Interfaces;
using Infrastructure.Data;
using Infrastructure.Notification;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories.Implementation.LINQRepository;
using Infrastructure.Persistence.Repositories.Implementation.SQLRepository;
using Infrastructure.Persistence.Repositories.Interfaces;
using Infrastructure.Services;
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
using Quartz;
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

        //Movie settings
        var tmdbSettings = new TmdbSettings();
        configuration.GetSection(TmdbSettings.SectionName).Bind(tmdbSettings);
        services.AddSingleton(Options.Create<TmdbSettings>(tmdbSettings));

        services.AddSignalR();
        //services.AddScoped<IHubContext<NotificationJob>>();

        services.AddQuartz(options =>
        {
            options.AddJob<NotificationJob>(JobKey.Create(nameof(NotificationJob)))
            .AddTrigger(trigger =>
            {
                trigger
                    .ForJob(JobKey.Create(nameof(NotificationJob)))
                    .StartNow()
                    .WithSimpleSchedule(s => s.WithIntervalInSeconds(30).RepeatForever());
            });
        });


        services.AddQuartzHostedService();

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
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IRatingService, RatingService>();

        if (configuration.GetValue<bool>("UseSql"))
        {
            services.AddScoped<IMovieRepository, SQLMovieRepository>();
            services.AddScoped<ICommentRepository, SQLCommentRepository>();
            services.AddScoped<IRatingRepository, SQLRatingsRepository>();
        }
        else
        {
            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
        }

        services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
        //services.AddScoped<IMovieService, MovieService>();
        var emailSettings = new EmailSettings();
        configuration.GetSection(EmailSettings.SectionName).Bind(emailSettings);
        services.AddSingleton(Options.Create<EmailSettings>(emailSettings));
    }


}
