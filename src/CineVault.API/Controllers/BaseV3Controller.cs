using CineVault.API.Controllers.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CineVault.API.Controllers;

[ApiController]
public abstract class BaseV3Controller : ControllerBase
{
    protected ActionResult<ApiResponse<T>> Ok<T>(T data, string? clientRequestId,
string message = "Task Successfull!")
 => Ok(new ApiResponse<T>
 {
     Success = true,
     Message = message,
     RequestId = clientRequestId,
     Data = data
 });

    protected ActionResult<ApiResponse<T>> Created<T>(T data, string?
clientRequestId, string message = "Object created!")
 => StatusCode(201, new ApiResponse<T>
 {
 Success = true,
 Message = message,
 RequestId = clientRequestId,
 Data = data
 });

    protected ActionResult<ApiResponse<object>> BadRequest(string?
clientRequestId, string message = "Bad Request")
=> BadRequest(new ApiResponse<object>
{
    Success = false,
    Message = message,
    RequestId = clientRequestId
});
    protected ActionResult<ApiResponse<object>> NotFound(string? clientRequestId,
   string message = "Object Not Found")
    => NotFound(new ApiResponse<object>
    {
        Success = false,
        Message = message,
        RequestId = clientRequestId
    });


}
