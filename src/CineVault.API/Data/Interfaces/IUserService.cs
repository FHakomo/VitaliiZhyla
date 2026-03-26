using CineVault.API.Controllers.Responses;
using CineVault.API.Controllers.Responses.MethodsExclusiveResponses;

namespace CineVault.API.Data.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserResponse>> SearchAsync(UserSearchRequest request);
}
