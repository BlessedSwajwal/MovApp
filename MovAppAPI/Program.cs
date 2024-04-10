using Infrastructure;
using Mapster;
using MapsterMapper;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
