namespace UniversitySystem3.Common.Exceptions;

public class ConflictException : AppException
{
    public ConflictException(string message, Exception? inner = null)
        : base(message, "Conflict", StatusCodes.Status409Conflict, inner) { }
}
