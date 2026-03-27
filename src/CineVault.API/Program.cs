using CineVault.API;
using CineVault.API.Controllers.Services;
using CineVault.API.Extensions;
using CineVault.API.Data.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using CineVault.API.Data.Entities;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.AddLogging();

builder.Services.AddCineVaultDbContext(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddRepositories();
builder.Services.AddApiVersioningWithApiExplorer();
builder.Services.AddSwaggerWithOptions();

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMapstter();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<UserService>();


var app = builder.Build();


var environment = builder.Environment.EnvironmentName;

Console.WriteLine($" === Запуск у середовищі: {environment} ===");

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithOptions();
}


// Виведення Developer Exception Page для локального середовища
if (app.Environment.IsLocal())
{
    app.UseDeveloperExceptionPage();
}
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CineVaultDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}
app.UseHttpsRedirection();
app.UseMiddleware<PerformanceLoggingMiddleware>();
app.MapControllers();

await app.RunAsync();