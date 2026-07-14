namespace LearningPlatform.Shared.Wrappers;

public class ApiResponse<T>
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];

    public static ApiResponse<T> Success(T data, string? message = null) =>
        new() { Succeeded = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message) =>
        new() { Succeeded = false, Message = message, Errors = [message] };

    public static ApiResponse<T> Fail(List<string> errors, string? message = null) =>
        new() { Succeeded = false, Message = message, Errors = errors };
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Success(string? message = null) =>
        new() { Succeeded = true, Message = message };

    public static new ApiResponse Fail(string message) =>
        new() { Succeeded = false, Message = message, Errors = [message] };

    public static new ApiResponse Fail(List<string> errors, string? message = null) =>
        new() { Succeeded = false, Message = message, Errors = errors };
}
