using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Quizzes.Interfaces;

public interface IQuizAuthorizationService
{
    /// <summary>Resolves the course that owns this quiz (directly, or via its lesson).</summary>
    Task<Course> GetQuizCourseAsync(Quiz quiz, CancellationToken cancellationToken = default);

    /// <summary>Throws ForbiddenException unless the current user is Admin or the owning instructor.</summary>
    Task<Course> EnsureCanManageQuizAsync(Quiz quiz, CancellationToken cancellationToken = default);

    /// <summary>
    /// Throws unless the current user may attempt/view this quiz: Admin, the owning
    /// instructor, or a student with the access the quiz's scope requires (free preview
    /// for lesson-scoped quizzes, otherwise an active, paid enrollment).
    /// </summary>
    Task<Course> EnsureCanTakeQuizAsync(Quiz quiz, CancellationToken cancellationToken = default);
}
