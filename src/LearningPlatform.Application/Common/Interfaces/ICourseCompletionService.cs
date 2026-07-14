namespace LearningPlatform.Application.Common.Interfaces;

/// <summary>
/// Grants official course completion (Enrollment → Completed, Certificate + PDF) once, when
/// the student is eligible: for courses with a final exam, only after passing it; for courses
/// without one, once lesson completion reaches 100%. No-op if not yet eligible or already
/// granted. Stages changes only; the caller's handler must still call SaveChangesAsync.
/// </summary>
public interface ICourseCompletionService
{
    Task TryGrantCourseCompletionAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
}
