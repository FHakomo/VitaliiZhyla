using Asp.Versioning;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Controllers.V3;
using CineVault.API.Data.Entities;
using CineVault.API.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CineVault.API.Controllers;
public class AppInfoControllerV3 : BaseV3Controller
{

    private readonly ILogger<UsersV3Controller> logger;

    public AppInfoControllerV3(ILogger<UsersV3Controller> logger)
    {
        this.logger = logger;
    }

    [ApiVersion(3.0)]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [HttpPost]
    public IActionResult GetEnvironmentV3([FromBody] ApiRequest request)
    {
        var data = new {Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
        MachineName = Environment.MachineName};

        this.logger.LogInformation("Environment were sent"); 
        return base.Ok(ApiResponse<object?>.Ok(data, request.RequestId, "Environment Information"));

    }

}
