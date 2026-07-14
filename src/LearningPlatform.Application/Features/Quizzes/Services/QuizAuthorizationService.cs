using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;

namespace LearningPlatform.Application.Features.Quizzes.Services;

public class QuizAuthorizationService(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    ILessonAccessService lessonAccessService)
    : IQuizAuthorizationService
{
    public async Task<Course> GetQuizCourseAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        if (quiz.CourseId.HasValue)
        {
            return await unitOfWork.Repository<Course>().GetByIdAsync(quiz.CourseId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(Course), quiz.CourseId.Value);
        }

        if (quiz.LessonId.HasValue)
        {
            var lesson = await unitOfWork.Repository<Lesson>().GetByIdAsync(quiz.LessonId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(Lesson), quiz.LessonId.Value);

            var section = await unitOfWork.Repository<CourseSection>().GetByIdAsync(lesson.CourseSectionId, cancellationToken)
                ?? throw new NotFoundException(nameof(CourseSection), lesson.CourseSectionId);

            return await unitOfWork.Repository<Course>().GetByIdAsync(section.CourseId, cancellationToken)
                ?? throw new NotFoundException(nameof(Course), section.CourseId);
        }

        throw new BadRequestException("This quiz is not attached to a course or lesson and cannot be managed here.");
    }

    public async Task<Course> EnsureCanManageQuizAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        var course = await GetQuizCourseAsync(quiz, cancellationToken);
        currentUser.EnsureCanManageCourse(course);
        return course;
    }

    public async Task<Course> EnsureCanTakeQuizAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        var course = await GetQuizCourseAsync(quiz, cancellationToken);

        if (currentUser.IsInRole(nameof(UserRole.Admin)) || currentUser.UserId == course.InstructorId)
            return course;

        if (quiz.LessonId.HasValue)
        {
            var lesson = await unitOfWork.Repository<Lesson>().GetByIdAsync(quiz.LessonId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(Lesson), quiz.LessonId.Value);

            await lessonAccessService.EnsureCanViewLessonAsync(course, lesson, cancellationToken);
            return course;
        }

        // Course-level (including the final exam): no free-preview concept, requires payment.
        if (currentUser.UserId is null)
            throw new UnauthorizedException("Sign in and enroll in this course to take this quiz.");

        var hasActiveEnrollment = await unitOfWork.Repository<Enrollment>().ExistsAsync(
            e => e.StudentId == currentUser.UserId && e.CourseId == course.Id && e.Status == EnrollmentStatus.Active,
            cancellationToken);

        if (!hasActiveEnrollment)
            throw new ForbiddenException("You must be enrolled in this course to take this quiz.");

        return course;
    }
}
