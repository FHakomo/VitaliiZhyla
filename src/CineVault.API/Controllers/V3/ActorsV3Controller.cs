using Asp.Versioning;
using CineVault.API.Controllers.Requests;
using CineVault.API.Controllers.Responses;
using CineVault.API.Controllers.Responses.MethodsExclusiveResponses;
using CineVault.API.Controllers.Services;
using CineVault.API.Data.Entities;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineVault.API.Controllers.V3;

[ApiVersion(3.0)]
[Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]/[action]")]
public class ActorsV3Controller : BaseV3Controller
{
    private readonly ILogger<ActorsV3Controller> logger;
    private readonly CineVaultDbContext dbContext;
    private readonly IMapper mapper;
    public ActorsV3Controller(CineVaultDbContext dbContext, ILogger<ActorsV3Controller> logger, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.logger = logger;
        this.mapper = mapper;
    }
    private static readonly Func<CineVaultDbContext, int, Task<Actor?>> GeActorByIdCompiled =
    EF.CompileAsyncQuery((CineVaultDbContext ctx, int id) =>
        ctx.Actors.AsNoTracking()
            .FirstOrDefault(m => m.Id == id));
    public async Task<ActionResult<ApiResponse<IEnumerable<ActorResponse>>>> GetActors([FromBody] ApiRequest request)
    {
        this.logger.LogInformation("Received request to get actors with RequestId: {RequestId}", request.RequestId);
        var comments = await this.dbContext.Actors.AsNoTracking().ToListAsync();
        var response = this.mapper.Map<IEnumerable<ActorResponse>>(comments);
        this.logger.LogInformation("Retrieved {ActorCount} movies", response.Count());
        return Ok(response, request.RequestId, "Movies got from Base");
    }
    [HttpPost("{id:int}")]
    public async Task<ActionResult<ApiResponse<ActorResponse>>> GetActorById(int id, [FromBody] ApiRequest request)
    {
        this.logger.LogInformation("Received request to get actor with ID {Actor} and RequestId: {RequestId}", id, request.RequestId);
        var actor = await GeActorByIdCompiled(this.dbContext, id);
        if (actor == null)
        {
            this.logger.LogWarning("Actor with id {CommentId} not found", id);
            return NotFound(ApiResponse<ActorResponse>.Fail($"Actor with id {id} not found", request.RequestId));
        }
        else
        {
            this.logger.LogInformation("Retrieved Actor with ID {CommentId}", id);
            return Ok(this.mapper.Map<ActorResponse>(actor), request.RequestId, $"Actor with id {id} got from Base");
        }
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ActorResponse>>> UpdateActor(int id, [FromBody] ApiRequest<ActorRequest> request)
    {
        var actor = await this.dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
        this.logger.LogInformation("Received request to update actor with RequestId: {RequestId}", request.RequestId);
        if (actor == null)
        {
            this.logger.LogWarning("Actor with id {ActorId} not found for update", id);
            return NotFound(ApiResponse<ActorResponse>.Fail($"Actor with id {id} not found", request.RequestId));
        }
        else
        {
            this.mapper.Map(request.Data, actor);
            this.logger.LogInformation("Actor with id {ActorId} found for update", id);
            await this.dbContext.SaveChangesAsync();
            return Ok(this.mapper.Map<ActorResponse>(actor), request.RequestId, $"Actor with id {actor.Id} updated successfully. RequestId = {request.RequestId}");
        }
    }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ActorResponse>>> CreateComment([FromBody] ApiRequest<ActorRequest> request)
    {
        this.logger.LogInformation("Received request to create actor with RequestId: {RequestId}", request.RequestId);
        var actor = this.mapper.Map<Actor>(request.Data);
        await this.dbContext.Actors.AddAsync(actor);
        await this.dbContext.SaveChangesAsync();
        return Ok(this.mapper.Map<ActorResponse>(actor), request.RequestId, $"Actor with id {actor.Id} created successfully. RequestId = {request.RequestId}");
    }
    [HttpPost("{id:int}/delete")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteComment(int id, [FromBody] ApiRequest<object?> request)
    {
        var actor = await this.dbContext.Actors.FirstOrDefaultAsync(c => c.Id == id);
        this.logger.LogInformation("Received request to delete actor with RequestId: {RequestId}", request.RequestId);
        if (actor == null)
        {
            this.logger.LogWarning("Actor with id {CommentId} not found for deletion", id);
            return NotFound(ApiResponse<object?>.Fail($"Actor with id {id} not found", request.RequestId));
        }
        else
        {
            actor.IsDeleted = true;
            await this.dbContext.SaveChangesAsync();
            return Ok<object?>(null!, ($"Actor with id {id} deleted successfully. RequestId = {request.RequestId}"));
        }
    }
    [HttpPost("bulk")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ActorResponse>>>> CreateActorsBulk([FromBody ] ApiRequest<IEnumerable<ActorRequest>> request)
    {

        this.logger.LogInformation("Request to create actors in bulk with RequestId: {RequestId}", request.RequestId);

        var actors = request.Adapt<List<Actor>>();
        foreach (var actor in actors)
        {
            if (string.IsNullOrWhiteSpace(actor.FullName))
            {
                logger.LogWarning("Skipping actor creation due to missing full name.");
                continue;
            }
            await dbContext.Actors.AddAsync(actor);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Created actor with title: {fullname}, ID: {ActorId}", actor.FullName, actor.Id);
        }

        var response = actors.Adapt<IEnumerable<ActorResponse>>();
        return Created(response, request.RequestId, $"Bulk actor creation completed.RequestId: {request.RequestId}");

    }
}
