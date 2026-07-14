using LearningPlatform.Application.Common.Interfaces;

namespace LearningPlatform.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
