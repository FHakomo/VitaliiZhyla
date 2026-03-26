using CineVault.API.Controllers.Responses;
using CineVault.API.Controllers.Responses.MethodsExclusiveResponses;
using CineVault.API.Data.Entities;
using CineVault.API.Data.Interfaces;
using CineVault.API.Data.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CineVault.API.Controllers.Services;

public class UserService : IUserService
{
    private readonly CineVaultDbContext dbContext;
    private readonly ILogger<UserService> logger;

    public UserService(CineVaultDbContext dbContext, ILogger<UserService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<PagedResult<UserResponse>> SearchAsync(UserSearchRequest request)
    {

        var query = dbContext.Users.AsQueryable();
        if (request.Username != null)
        {
            query = query.Where(u => u.Username.Contains(request.Username));
        }
        if (request.Email != null)
        {
            query = query.Where(u => u.Email.Contains(request.Email));
        }
        query = request.SortBy switch
        {
            "email" => query.OrderBy(u => u.Email),
            "" => query.OrderBy(u => u.Username)
        };
        var total = await query.CountAsync();
        var items = await query.Skip((request.Page - 1) * request.PageSize.Value).Take(request.PageSize.Value).ToListAsync();
        logger.LogInformation("User search with {Filters}, total {Total}", request, total);
        return new PagedResult<UserResponse>
        {
            Items = items.Adapt<List<UserResponse>>(),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize ?? total
        };
    }
}