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

        var enrolledCourseIds = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .Where(e => e.StudentId == currentUser.UserId && e.Status == EnrollmentStatus.Active)
            .Select(e => e.CourseId)
            .ToListAsync(cancellationToken);

        return baseQuery.Where(s => enrolledCourseIds.Contains(s.CourseId));
    }
}
