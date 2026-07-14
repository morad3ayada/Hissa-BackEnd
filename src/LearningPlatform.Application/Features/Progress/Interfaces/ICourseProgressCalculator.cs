using LearningPlatform.Application.Features.Progress.DTOs;

namespace LearningPlatform.Application.Features.Progress.Interfaces;

public interface ICourseProgressCalculator
{
    Task<CourseProgressSummaryDto> CalculateSummaryAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default);

    Task<CourseProgressDetailDto> CalculateDetailAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default);
}
