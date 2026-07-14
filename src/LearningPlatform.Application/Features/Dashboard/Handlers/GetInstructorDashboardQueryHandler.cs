using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Dashboard.DTOs;
using LearningPlatform.Application.Features.Dashboard.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Dashboard.Handlers;

public class GetInstructorDashboardQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetInstructorDashboardQuery, ApiResponse<InstructorDashboardDto>>
{
    private const int TopN = 5;

    public async Task<ApiResponse<InstructorDashboardDto>> Handle(GetInstructorDashboardQuery request, CancellationToken cancellationToken)
    {
        var instructorId = currentUser.UserId!.Value;

        var courseIds = await unitOfWork.Repository<Course>().AsQueryable()
            .Where(c => c.InstructorId == instructorId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var studentsCount = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .Where(e => courseIds.Contains(e.CourseId))
            .Select(e => e.StudentId)
            .Distinct()
            .CountAsync(cancellationToken);

        var averageCourseRating = await unitOfWork.Repository<Review>().AsQueryable()
            .Where(r => courseIds.Contains(r.CourseId))
            .Select(r => (decimal?)r.Rating)
            .AverageAsync(cancellationToken) ?? 0;

        var totalEnrollments = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .CountAsync(e => courseIds.Contains(e.CourseId), cancellationToken);

        var completedEnrollments = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .CountAsync(e => courseIds.Contains(e.CourseId) && e.Status == EnrollmentStatus.Completed, cancellationToken);

        var completionRate = totalEnrollments == 0 ? 0 : Math.Round(completedEnrollments * 100m / totalEnrollments, 2);

        var mostWatchedLessons = await unitOfWork.Repository<CourseProgress>().AsQueryable()
            .Where(p => courseIds.Contains(p.Lesson.CourseSection.CourseId))
            .GroupBy(p => new { p.LessonId, LessonTitle = p.Lesson.Title, CourseTitle = p.Lesson.CourseSection.Course.Title })
            .OrderByDescending(g => g.Count())
            .Take(TopN)
            .Select(g => new TopLessonDto
            {
                LessonId = g.Key.LessonId,
                Title = g.Key.LessonTitle,
                CourseTitle = g.Key.CourseTitle,
                ViewCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        var quizIds = await unitOfWork.Repository<Quiz>().AsQueryable()
            .Where(q => (q.CourseId.HasValue && courseIds.Contains(q.CourseId.Value)) ||
                        (q.LessonId.HasValue && courseIds.Contains(q.Lesson!.CourseSection.CourseId)))
            .Select(q => q.Id)
            .ToListAsync(cancellationToken);

        var totalAttempts = await unitOfWork.Repository<QuizResult>().AsQueryable()
            .CountAsync(r => quizIds.Contains(r.QuizId), cancellationToken);

        var passedCount = await unitOfWork.Repository<QuizResult>().AsQueryable()
            .CountAsync(r => quizIds.Contains(r.QuizId) && r.IsPassed, cancellationToken);

        var quizResultsSummary = new QuizResultsSummaryDto
        {
            TotalAttempts = totalAttempts,
            PassedCount = passedCount,
            PassRate = totalAttempts == 0 ? 0 : Math.Round(passedCount * 100m / totalAttempts, 2)
        };

        var now = DateTime.UtcNow;
        var upcomingLiveSessions = await unitOfWork.Repository<LiveSession>().AsQueryable()
            .Where(s => s.InstructorId == instructorId && s.EndDateTime >= now && s.Status != LiveSessionStatus.Cancelled)
            .OrderBy(s => s.StartDateTime)
            .Take(TopN)
            .Select(s => new UpcomingSessionDto
            {
                Id = s.Id,
                Title = s.Title,
                CourseTitle = s.Course.Title,
                StartDateTime = s.StartDateTime
            })
            .ToListAsync(cancellationToken);

        var dto = new InstructorDashboardDto
        {
            CoursesCount = courseIds.Count,
            StudentsCount = studentsCount,
            AverageCourseRating = Math.Round(averageCourseRating, 2),
            StudentCompletionRate = completionRate,
            MostWatchedLessons = mostWatchedLessons,
            QuizResultsSummary = quizResultsSummary,
            UpcomingLiveSessions = upcomingLiveSessions
        };

        return ApiResponse<InstructorDashboardDto>.Success(dto);
    }
}
