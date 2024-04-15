using Infrastructure;
using Infrastructure.Services.Implementation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddInfrastructure(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           SharedFile.GetSharedFolderPath()),
    RequestPath = "/Images"
});

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}").RequireAuthorization();

app.Run();
