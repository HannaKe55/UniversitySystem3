using System.Runtime.CompilerServices;

namespace UniversitySystem3.Common.Exceptions;

public abstract class AppException : Exception
{
    public string ErrorType { get; }
    public int StatusCode { get; }
    public string SourceClass { get; }
    public string SourceMethod { get; }
    public string SourceFile { get; }
    public int SourceLine { get; }

    protected AppException(
        string message,
        string errorType,
        int statusCode,
        Exception? innerException = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        : base(message, innerException)
    {
        ErrorType = errorType;
        StatusCode = statusCode;
        SourceMethod = memberName;
        SourceFile = Path.GetFileNameWithoutExtension(filePath);
        SourceLine = lineNumber;
        SourceClass = SourceFile; // فایل و کلاس معمولاً هم‌نامن توی این پروژه
    }
}
