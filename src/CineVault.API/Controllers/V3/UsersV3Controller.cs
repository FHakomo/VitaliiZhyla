using Asp.Versioning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Data.Interfaces;
using CineVault.API.Data.Entities;
using Mapster;


namespace CineVault.API.Controllers.V3;

[ApiVersion(3.0)]
[Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]/[action]")]
public class UsersV3Controller : BaseV3Controller
{
    private readonly IUserRepository userRepository;
    private readonly ILogger<UsersV3Controller> logger;

    public UsersV3Controller(IUserRepository userRepository, ILogger<UsersV3Controller> logger)
    {
        this.userRepository = userRepository;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetUsers([FromBody] ApiRequest request)
    {
        this.logger.LogInformation("Received request to get users with RequestId: {RequestId}", request.RequestId);
        var users = await this.userRepository.GetAll();
        var response = users.Adapt<IEnumerable<UserResponse>>();
        this.logger.LogInformation("Retrieved {UserCount} users", response.Count());
        return Ok(response, request.RequestId, "Users got from Base" );
    }
    [HttpPost("{id:int}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUserById(int id, [FromBody] ApiRequest request)
    {
        this.logger.LogInformation("Received request to get user with ID {UserId} and RequestId: {RequestId}", id, request.RequestId);
        var user = await this.userRepository.GetById(id);
        if (user == null)
        {
            this.logger.LogWarning("User with id {UserId} not found", id);
            return NotFound(ApiResponse<UserResponse>.Fail($"User with id {id} not found", request.RequestId));
        }
        else
        {
            this.logger.LogInformation("Retrieved user with ID {UserId}", id);
            return Ok(user.Adapt<UserResponse>(), request.RequestId, $"User with id {id} got from Base");
        }
    }   
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateUser(int id, [FromBody] ApiRequest<UserRequest> request)
    {
        var user = await this.userRepository.GetById(id);
        this.logger.LogInformation("Received request to update user with RequestId: {RequestId}", request.RequestId);

        if (user == null)
        {
            this.logger.LogWarning("User with id {UserId} not found for update", id);
            return NotFound(ApiResponse<UserResponse>.Fail($"User with id {id} not found", request.RequestId));
        }
        else
        {
            request.Data.Adapt(user);
            { this.logger.LogInformation("User with id {UserId} found for update", id); }
            await this.userRepository.Update(user);
            return Ok(user.Adapt<UserResponse>(), request.RequestId, $"User with id {user.Id} updated successfully. RequestId = {request.RequestId}");
        }
            

    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateUser([FromBody] ApiRequest<UserRequest> request)
    {
        this.logger.LogInformation("Received request to create user with RequestId: {RequestId}", request.RequestId);
        var user = request.Data.Adapt<User>();
        if (user == null)
        {
            this.logger.LogWarning("Invalid user data provided for creation. RequestId: {RequestId}", request.RequestId);
            return BadRequest(ApiResponse<UserResponse>.Fail("Invalid user data", request.RequestId));
        }
        await this.userRepository.Create(user);
        this.logger.LogInformation("Created new user with ID {UserId}. RequestId: {RequestId}", user.Id, request.RequestId);
        return Ok(user.Adapt<UserResponse>(), request.RequestId, $"User with id {user.Id} created successfully. RequestId = {request.RequestId}");
    }
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteUser(
        int id, [FromBody] ApiRequest<object?> request)
    {
        logger.LogInformation("Deleting user {UserId} (v3). RequestId: {RequestId}", id, request.RequestId);
        var user = await userRepository.GetById(id);
        if (user is null)
        {
            return NotFound(ApiResponse<object?>.Fail(request.RequestId, $"User with id {id} not found"));
        }
        await userRepository.Delete(user);
        return Ok<object?>(null, request.RequestId, "User deleted successfully");
    }
}

