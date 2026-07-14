using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Dashboard.DTOs;
using LearningPlatform.Application.Features.Dashboard.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Dashboard.Handlers;

public class GetAdminDashboardQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    : IRequestHandler<GetAdminDashboardQuery, ApiResponse<AdminDashboardDto>>
{
    private const int TopN = 5;
    private const int ActiveWindowDays = 30;

    public async Task<ApiResponse<AdminDashboardDto>> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        var courseRepository = unitOfWork.Repository<Course>();

        var totalUsers = await userManager.Users.CountAsync(cancellationToken);
        var totalStudents = await userManager.Users.CountAsync(u => u.Role == UserRole.Student, cancellationToken);
        var totalInstructors = await userManager.Users.CountAsync(u => u.Role == UserRole.Instructor, cancellationToken);
        var totalParents = await userManager.Users.CountAsync(u => u.Role == UserRole.Parent, cancellationToken);

        var totalCourses = await courseRepository.AsQueryable().CountAsync(cancellationToken);
        var publishedCourses = await courseRepository.AsQueryable().CountAsync(c => c.Status == CourseStatus.Published, cancellationToken);
        var pendingCourses = await courseRepository.AsQueryable().CountAsync(c => c.Status == CourseStatus.PendingReview, cancellationToken);
        var rejectedCourses = await courseRepository.AsQueryable().CountAsync(c => c.Status == CourseStatus.Rejected, cancellationToken);

        var totalEnrollments = await unitOfWork.Repository<Enrollment>().AsQueryable().CountAsync(cancellationToken);

        var paymentsByStatus = await unitOfWork.Repository<Payment>().AsQueryable()
            .GroupBy(p => p.Status)
            .Select(g => new PaymentStatusBreakdownDto
            {
                Status = g.Key.ToString(),
                Count = g.Count(),
                TotalAmount = g.Sum(p => p.Amount)
            })
            .ToListAsync(cancellationToken);

        var topEnrolledCourses = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .GroupBy(e => new { e.CourseId, e.Course.Title })
            .OrderByDescending(g => g.Count())
            .Take(TopN)
            .Select(g => new TopCourseDto { CourseId = g.Key.CourseId, Title = g.Key.Title, Count = g.Count() })
            .ToListAsync(cancellationToken);

        // "Views" proxy: distinct lesson-watch progress rows logged per course.
        var topViewedCourses = await unitOfWork.Repository<CourseProgress>().AsQueryable()
            .GroupBy(p => new { p.Lesson.CourseSection.CourseId, p.Lesson.CourseSection.Course.Title })
            .OrderByDescending(g => g.Count())
            .Take(TopN)
            .Select(g => new TopCourseDto { CourseId = g.Key.CourseId, Title = g.Key.Title, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var mostActiveInstructors = await GetMostActiveInstructorsAsync(cancellationToken);

        var totalQuizzes = await unitOfWork.Repository<Quiz>().AsQueryable().CountAsync(cancellationToken);
        var totalLiveSessions = await unitOfWork.Repository<LiveSession>().AsQueryable().CountAsync(cancellationToken);

        var totalQuizAttempts = await unitOfWork.Repository<QuizResult>().AsQueryable().CountAsync(cancellationToken);
        var passedQuizAttempts = await unitOfWork.Repository<QuizResult>().AsQueryable().CountAsync(r => r.IsPassed, cancellationToken);
        var averagePassRate = totalQuizAttempts == 0 ? 0 : Math.Round(passedQuizAttempts * 100m / totalQuizAttempts, 2);

        var activeSince = DateTime.UtcNow.AddDays(-ActiveWindowDays);
        var activeUsersCount = await unitOfWork.Repository<RefreshToken>().AsQueryable()
            .Where(t => t.CreatedAt >= activeSince)
            .Select(t => t.UserId)
            .Distinct()
            .CountAsync(cancellationToken);

        var dto = new AdminDashboardDto
        {
            TotalUsers = totalUsers,
            TotalStudents = totalStudents,
            TotalInstructors = totalInstructors,
            TotalParents = totalParents,
            TotalCourses = totalCourses,
            PublishedCourses = publishedCourses,
            PendingCourses = pendingCourses,
            RejectedCourses = rejectedCourses,
            TotalEnrollments = totalEnrollments,
            PaymentsByStatus = paymentsByStatus,
            TopEnrolledCourses = topEnrolledCourses,
            TopViewedCourses = topViewedCourses,
            MostActiveInstructors = mostActiveInstructors,
            TotalQuizzes = totalQuizzes,
            TotalLiveSessions = totalLiveSessions,
            AveragePassRate = averagePassRate,
            ActiveUsersCount = activeUsersCount
        };

        return ApiResponse<AdminDashboardDto>.Success(dto);
    }

    // Split into two flat, EF-translatable queries (course counts, distinct-student counts) and
    // merged client-side — a single GroupBy with a nested SelectMany(...).Distinct().Count()
    // aggregate does not reliably translate to SQL.
    private async Task<List<TopInstructorDto>> GetMostActiveInstructorsAsync(CancellationToken cancellationToken)
    {
        var courseCounts = await unitOfWork.Repository<Course>().AsQueryable()
            .GroupBy(c => c.InstructorId)
            .Select(g => new { InstructorId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var studentCounts = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .Select(e => new { e.Course.InstructorId, e.StudentId })
            .Distinct()
            .GroupBy(x => x.InstructorId)
            .Select(g => new { InstructorId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var instructorIds = courseCounts.Select(x => x.InstructorId)
            .Union(studentCounts.Select(x => x.InstructorId))
            .ToList();

        var instructorNames = await userManager.Users
            .Where(u => instructorIds.Contains(u.Id))
            .Select(u => new { u.Id, Name = u.FirstName + " " + u.LastName })
            .ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);

        return instructorIds
            .Select(id => new TopInstructorDto
            {
                InstructorId = id,
                Name = instructorNames.GetValueOrDefault(id, "Unknown"),
                CoursesCount = courseCounts.FirstOrDefault(x => x.InstructorId == id)?.Count ?? 0,
                StudentsCount = studentCounts.FirstOrDefault(x => x.InstructorId == id)?.Count ?? 0
            })
            .OrderByDescending(x => x.StudentsCount)
            .Take(TopN)
            .ToList();
    }
}
