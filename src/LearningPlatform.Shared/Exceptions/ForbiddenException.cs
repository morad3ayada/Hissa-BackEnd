using System.Net;

namespace LearningPlatform.Shared.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message, HttpStatusCode.Forbidden)
    {
    }
}
