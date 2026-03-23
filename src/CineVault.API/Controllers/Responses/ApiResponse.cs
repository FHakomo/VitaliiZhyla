namespace CineVault.API.Controllers.Responses;

public class ApiResponse
{

    public required bool Success { get; set; }
 public required string Message { get; set; }
 public DateTime Timestamp { get; set; } = DateTime.UtcNow;
 public required string RequestId { get; set; }
 public Dictionary<string, object>? Metadata { get; set; }

}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string requestId, string message = "Success")
 => new() { Success = true, Data = data, Message = message, RequestId = requestId};

    public static ApiResponse<T> Fail(string message, string requestId)
 => new() { Success = false, Message = message, RequestId=requestId };
}
