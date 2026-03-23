using CineVault.API;
using CineVault.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;
[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.AddLogging();

builder.Services.AddCineVaultDbContext(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddRepositories();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


var environment = builder.Environment.EnvironmentName;

Console.WriteLine($" === Запуск у середовищі: {environment} ===");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Виведення Developer Exception Page для локального середовища
if (app.Environment.IsLocal())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseMiddleware<PerformanceLoggingMiddleware>();
app.MapControllers();

await app.RunAsync();