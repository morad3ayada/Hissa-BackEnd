namespace LearningPlatform.Application.Common.Interfaces;

public interface ICourseDurationRecalculator
{
    /// <summary>
    /// Recomputes Course.DurationInMinutes as the sum of every lesson's duration
    /// across all of the course's sections. Call after any lesson create/update/delete
    /// or video upload/replace/delete that could change a lesson's duration.
    /// </summary>
    Task RecalculateAsync(Guid courseId, CancellationToken cancellationToken = default);
}
