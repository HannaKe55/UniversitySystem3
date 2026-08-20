namespace UniversitySystem3.Services.Common;

public enum ServiceResultType
{
    Ok,
    BadRequest,
    NotFound,
    Forbidden
}

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public T? Data { get; set; }
    public ServiceResultType ResultType { get; set; } = ServiceResultType.Ok;

    public static ServiceResult<T> Ok(T data) =>
        new() { Success = true, Data = data, ResultType = ServiceResultType.Ok };

    public static ServiceResult<T> Fail(string message, ServiceResultType type = ServiceResultType.BadRequest) =>
        new() { Success = false, ErrorMessage = message, ResultType = type };
}