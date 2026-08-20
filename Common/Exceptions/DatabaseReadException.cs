namespace UniversitySystem3.Common.Exceptions;

public class DatabaseReadException : AppException
{
    public DatabaseReadException(string message, Exception? inner = null)
        : base(message, "DatabaseReadError", StatusCodes.Status500InternalServerError, inner) { }
}
