namespace UniversitySystem3.Common.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "دسترسی غیرمجاز.")
        : base(message, "Forbidden", StatusCodes.Status403Forbidden) { }
}
