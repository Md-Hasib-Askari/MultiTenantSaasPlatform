namespace Api.Models;

public record ApiResponse<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    public static ApiResponse<T> Ok(T? data, string? message = null) => new()
    {
        Success = true,
        Message = message,
        Data = data
    };

    public static ApiResponse<T> Ok(string? message = null) => new()
    {
        Success = true,
        Message = message
    };

    public static ApiResponse<T> Fail(string message) => new()
    {
        Success = false,
        Message = message
    };
}
