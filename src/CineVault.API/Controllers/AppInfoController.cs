using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CineVault.API.Controllers;
public class AppInfoController : Controller
{
    [ApiVersion(1.0, Deprecated = true)]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [HttpGet]
    public IActionResult GetEnvironment()
    {
        // Отримуємо значення змінної середовища ASPNETCORE_ENVIRONMENT
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
        // Повертаємо значення у відповіді
        return Ok(new { Environment = environment });
    }

}
