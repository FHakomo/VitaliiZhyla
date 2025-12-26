using CineVault.API.Extensions;
using Microsoft.AspNetCore.Mvc;
[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCineVaultDbContext(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddRepositories();


builder.Services.AddApiVersioningWithApiExplorer();
builder.Services.AddSwaggerWithOptions();


var app = builder.Build();

var environment = builder.Environment.EnvironmentName;

Console.WriteLine($" === Запуск у середовищі: {environment} ===");

if (app.Environment.IsDevelopment())
{
   app.UseSwaggerWithOptions();
}


// Виведення Developer Exception Page для локального середовища
if (app.Environment.isLocal())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();