using System.Net;

namespace LearningPlatform.Shared.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
}
