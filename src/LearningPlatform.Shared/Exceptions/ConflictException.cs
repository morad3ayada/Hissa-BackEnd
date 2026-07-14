using System.Net;

namespace LearningPlatform.Shared.Exceptions;

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, HttpStatusCode.Conflict)
    {
    }
}
