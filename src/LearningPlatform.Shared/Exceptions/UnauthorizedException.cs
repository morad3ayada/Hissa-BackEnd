using System.Net;

namespace LearningPlatform.Shared.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "You are not authorized to perform this action.")
        : base(message, HttpStatusCode.Unauthorized)
    {
    }
}
