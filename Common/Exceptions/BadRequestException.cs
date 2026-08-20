namespace UniversitySystem3.Common.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string message)
        : base(message, "BadRequest", StatusCodes.Status400BadRequest) { }

}
