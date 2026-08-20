namespace UniversitySystem3.Common.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, "NotFound", StatusCodes.Status404NotFound) { }
}
