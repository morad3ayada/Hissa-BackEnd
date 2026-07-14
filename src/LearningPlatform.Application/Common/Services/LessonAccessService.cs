using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;

namespace LearningPlatform.Application.Common.Services;

public class LessonAccessService(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : ILessonAccessService
{
    public async Task EnsureCanViewLessonAsync(Course course, Lesson lesson, CancellationToken cancellationToken = default)
    {
        if (currentUser.IsInRole(nameof(UserRole.Admin)))
            return;

        if (currentUser.UserId == course.InstructorId)
            return;

        if (lesson.IsFreePreview)
            return;

        if (currentUser.UserId is null)
            throw new UnauthorizedException("Sign in and enroll in this course to view this lesson.");

        var enrollments = await unitOfWork.Repository<Enrollment>().FindAsync(
            e => e.StudentId == currentUser.UserId && e.CourseId == course.Id, cancellationToken);
        var enrollment = enrollments.FirstOrDefault();

        if (enrollment is null || enrollment.Status != EnrollmentStatus.Active)
            throw new ForbiddenException("You must be enrolled in this course to view this lesson.");

        // Defense in depth: Active should only ever be reached via an approved (Completed)
        // payment, but verify directly in case that invariant is ever broken elsewhere.
        var hasCompletedPayment = await unitOfWork.Repository<Payment>().ExistsAsync(
            p => p.EnrollmentId == enrollment.Id && p.Status == PaymentStatus.Completed, cancellationToken);

        if (!hasCompletedPayment)
            throw new ForbiddenException("Your payment for this course has not been confirmed yet.");
    }
}
