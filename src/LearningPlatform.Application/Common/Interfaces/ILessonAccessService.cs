using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Common.Interfaces;

public interface ILessonAccessService
{
    /// <summary>
    /// Throws ForbiddenException unless the current user may view this lesson:
    /// Admin, the owning instructor, anyone when the lesson is a free preview,
    /// or a student with an active enrollment in the parent course.
    /// </summary>
    Task EnsureCanViewLessonAsync(Course course, Lesson lesson, CancellationToken cancellationToken = default);
}
