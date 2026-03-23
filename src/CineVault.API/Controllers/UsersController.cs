using Asp.Versioning;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineVault.API.Controllers;

[ApiVersion(1.0, Deprecated = true)]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly ILogger<UsersController> logger;

    public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
    {
        this.userRepository = userRepository;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        var users = await this.userRepository.GetAll();

        var response = users.Select(UserResponse.FromEntity);

        logger.LogInformation("Retrieved {UsersCount} users", response.Count());
        return base.Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUserById(int id)
    {
        var user = await this.userRepository.GetById(id);

        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found", id);
            return base.NotFound();
        }

        logger.LogInformation("Retrieved user with ID {UserId}", id);
        return base.Ok(UserResponse.FromEntity(user));
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(UserRequest request)
    {
        var user = request.ToEntity();

        await this.userRepository.Create(user);

        logger.LogInformation("Created new user with ID {UserId}", user.Id);
        return base.Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, UserRequest request)
    {
        var user = await this.userRepository.GetById(id);

        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found for update", id);
            return base.NotFound();
        }

        request.ApplyTo(user);

        await this.userRepository.Update(user);

        logger.LogInformation("Updated user with ID {UserId}", id);
        return base.Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var user = await this.userRepository.GetById(id);

        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found for deletion", id);
            return base.NotFound();
        }

        await this.userRepository.Delete(user);

        logger.LogInformation("Deleted user with ID {UserId}", id);
        return base.NoContent();
    }
}