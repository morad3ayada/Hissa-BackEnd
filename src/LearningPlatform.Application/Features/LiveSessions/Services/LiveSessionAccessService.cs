using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.LiveSessions.Services;

public class LiveSessionAccessService(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : ILiveSessionAccessService
{
    public async Task EnsureCanViewAsync(LiveSession session, CancellationToken cancellationToken = default)
    {
        if (currentUser.IsInRole(nameof(UserRole.Admin)) || currentUser.UserId == session.InstructorId)
            return;

        if (currentUser.UserId is null)
            throw new UnauthorizedException("Sign in to view this live session.");

        if (await CanViewAsParentAsync(session.CourseId, cancellationToken))
            return;

        var isEnrolled = await unitOfWork.Repository<Enrollment>().ExistsAsync(
            e => e.StudentId == currentUser.UserId && e.CourseId == session.CourseId && e.Status == EnrollmentStatus.Active,
            cancellationToken);

        if (!isEnrolled)
            throw new ForbiddenException("You must be enrolled in this course to view its live sessions.");
    }

    public async Task EnsureCanViewCourseSessionsAsync(Course course, CancellationToken cancellationToken = default)
    {
        if (currentUser.IsInRole(nameof(UserRole.Admin)) || currentUser.UserId == course.InstructorId)
            return;

        if (currentUser.UserId is null)
            throw new UnauthorizedException("Sign in to view this course's live sessions.");

        if (await CanViewAsParentAsync(course.Id, cancellationToken))
            return;

        var isEnrolled = await unitOfWork.Repository<Enrollment>().ExistsAsync(
            e => e.StudentId == currentUser.UserId && e.CourseId == course.Id && e.Status == EnrollmentStatus.Active,
            cancellationToken);

        if (!isEnrolled)
            throw new ForbiddenException("You must be enrolled in this course to view its live sessions.");
    }

    public async Task<IQueryable<LiveSession>> GetVisibleSessionsQueryAsync(CancellationToken cancellationToken = default)
    {
        IQueryable<LiveSession> baseQuery = unitOfWork.Repository<LiveSession>().AsQueryable()
            .Include(s => s.Course)
            .Include(s => s.Instructor);

        if (currentUser.IsInRole(nameof(UserRole.Admin)))
            return baseQuery;

        if (currentUser.IsInRole(nameof(UserRole.Instructor)))
            return baseQuery.Where(s => s.InstructorId == currentUser.UserId);

        var userId = currentUser.UserId!;

        if (currentUser.IsInRole(nameof(UserRole.Parent)))
        {
            var childIds = await unitOfWork.Repository<ParentStudent>().AsQueryable()
                .Where(ps => ps.ParentId == userId)
                .Select(ps => ps.StudentId)
                .ToListAsync(cancellationToken);

            var parentCourseIds = await unitOfWork.Repository<Enrollment>().AsQueryable()
                .Where(e => childIds.Contains(e.StudentId) && e.Status == EnrollmentStatus.Active)
                .Select(e => e.CourseId)
                .ToListAsync(cancellationToken);

            return baseQuery.Where(s => parentCourseIds.Contains(s.CourseId));
        }

        var enrolledCourseIds = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .Where(e => e.StudentId == userId && e.Status == EnrollmentStatus.Active)
            .Select(e => e.CourseId)
            .ToListAsync(cancellationToken);

        return baseQuery.Where(s => enrolledCourseIds.Contains(s.CourseId));
    }

    private async Task<bool> CanViewAsParentAsync(Guid courseId, CancellationToken cancellationToken)
    {
        if (!currentUser.IsInRole(nameof(UserRole.Parent)) || currentUser.UserId is null)
            return false;

        var childIds = await unitOfWork.Repository<ParentStudent>().AsQueryable()
            .Where(ps => ps.ParentId == currentUser.UserId)
            .Select(ps => ps.StudentId)
            .ToListAsync(cancellationToken);

        return await unitOfWork.Repository<Enrollment>().ExistsAsync(
            e => childIds.Contains(e.StudentId) && e.CourseId == courseId && e.Status == EnrollmentStatus.Active,
            cancellationToken);
    }
}
