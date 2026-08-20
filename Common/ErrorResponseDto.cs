namespace UniversitySystem3.Common;

public class ErrorResponseDto
{
    public string Message { get; set; } = null!;
    public string ErrorType { get; set; } = null!;
    public int StatusCode { get; set; }
    public string Source { get; set; } = null!;   // Namespace.Class.Method
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
