using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace CineVault.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1.0, Deprecated = true)]
[ApiVersion(2.0)]
public class AppInfoController : ControllerBase
{
    [HttpGet("environment")]
    [MapToApiVersion(1.0)]
    public IActionResult GetEnvironmentV1()
    {
        // Отримуємо значення змінної середовища ASPNETCORE_ENVIRONMENT
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
        // Повертаємо значення у відповіді
        return Ok(new { Environment = environment });
    }

    [HttpGet("environment")]
    [MapToApiVersion(2.0)]
    public IActionResult GetEnvironmentV2()
    {
        // Той самий процес як і в попередньому, але з версією
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
        return Ok(new { Environment = environment, Response = "V2" });
    }
}