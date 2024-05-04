using Infrastructure;
using Infrastructure.Notification;
using Infrastructure.Services.Implementation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.WithOrigins(["http://localhost:5173"]).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

{
    //Mapster
    builder.Services.AddSingleton<IMapper, ServiceMapper>();
    var config = TypeAdapterConfig.GlobalSettings;
    config.Scan(typeof(Program).Assembly);
    config.Scan(typeof(Infrastructure.DependencyRegister).Assembly);

    builder.Services.AddSingleton(config);
}



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyPolicy");

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           SharedFile.GetSharedFolderPath()),
    RequestPath = "/Images"
});

app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hub/notifications");
app.MapHub<CommentUpdateHub>("/hub/comments");

app.Run();
