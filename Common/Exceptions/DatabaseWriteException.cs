namespace UniversitySystem3.Common.Exceptions;

public class DatabaseWriteException : AppException
{
    public DatabaseWriteException(string message, Exception? inner = null)
       : base(message, "DatabaseWriteError", StatusCodes.Status500InternalServerError, inner) { }

}
