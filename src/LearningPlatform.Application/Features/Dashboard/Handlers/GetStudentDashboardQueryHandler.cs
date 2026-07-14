using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Dashboard.DTOs;
using LearningPlatform.Application.Features.Dashboard.Queries;
using LearningPlatform.Application.Features.Progress.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Dashboard.Handlers;

public class GetStudentDashboardQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, ICourseProgressCalculator courseProgressCalculator)
    : IRequestHandler<GetStudentDashboardQuery, ApiResponse<StudentDashboardDto>>
{
    private const int TopN = 5;

    public async Task<ApiResponse<StudentDashboardDto>> Handle(GetStudentDashboardQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        // "My courses" = Active or Completed — a finished course is still one the student is
        // enrolled in; only Cancelled/Expired/PendingPayment enrollments are excluded.
        var myEnrollments = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .Where(e => e.StudentId == studentId && (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed))
            .Select(e => new { e.CourseId, e.Course.Title })
            .ToListAsync(cancellationToken);

        var enrolledCourses = new List<EnrolledCourseSummaryDto>();
        foreach (var enrollment in myEnrollments)
        {
            var summary = await courseProgressCalculator.CalculateSummaryAsync(enrollment.CourseId, studentId, cancellationToken);
            enrolledCourses.Add(new EnrolledCourseSummaryDto
            {
                CourseId = enrollment.CourseId,
                Title = enrollment.Title,
                ProgressPercentage = summary.CompletionPercentage
            });
        }

        var overallProgress = enrolledCourses.Count == 0 ? 0 : Math.Round(enrolledCourses.Average(c => c.ProgressPercentage), 2);

        var lastProgress = await unitOfWork.Repository<CourseProgress>().AsQueryable()
            .Include(p => p.Lesson).ThenInclude(l => l.CourseSection).ThenInclude(s => s.Course)
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.LastWatchedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var lastLesson = lastProgress is null ? null : new LastLessonDto
        {
            LessonId = lastProgress.LessonId,
            Title = lastProgress.Lesson.Title,
            CourseTitle = lastProgress.Lesson.CourseSection.Course.Title,
            LastWatchedAt = lastProgress.LastWatchedAt ?? lastProgress.CreatedAt
        };

        var lastQuizResult = await unitOfWork.Repository<QuizResult>().AsQueryable()
            .Include(r => r.Quiz)
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CompletedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var lastQuiz = lastQuizResult is null ? null : new LastQuizDto
        {
            QuizId = lastQuizResult.QuizId,
            Title = lastQuizResult.Quiz.Title,
            Score = lastQuizResult.Score,
            IsPassed = lastQuizResult.IsPassed,
            CompletedAt = lastQuizResult.CompletedAt ?? lastQuizResult.CreatedAt
        };

        var gamificationProfile = (await unitOfWork.Repository<GamificationProfile>()
            .FindAsync(p => p.StudentId == studentId, cancellationToken)).FirstOrDefault();

        var achievementsCount = await unitOfWork.Repository<StudentReward>().AsQueryable()
            .CountAsync(sr => sr.StudentId == studentId && sr.Reward.TriggerType != null, cancellationToken);

        var certificatesCount = await unitOfWork.Repository<Certificate>().AsQueryable()
            .CountAsync(c => c.StudentId == studentId, cancellationToken);

        // Upcoming sessions only matter for courses still actively being taken.
        var activeCourseIds = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Active)
            .Select(e => e.CourseId)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var upcomingLiveSessions = await unitOfWork.Repository<LiveSession>().AsQueryable()
            .Where(s => activeCourseIds.Contains(s.CourseId) && s.EndDateTime >= now && s.Status != LiveSessionStatus.Cancelled)
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

        var dto = new StudentDashboardDto
        {
            EnrolledCourses = enrolledCourses,
            OverallProgressPercentage = overallProgress,
            LastLesson = lastLesson,
            LastQuiz = lastQuiz,
            Points = gamificationProfile?.TotalPoints ?? 0,
            Level = gamificationProfile?.CurrentLevel ?? 1,
            AchievementsCount = achievementsCount,
            CertificatesCount = certificatesCount,
            UpcomingLiveSessions = upcomingLiveSessions
        };

        return ApiResponse<StudentDashboardDto>.Success(dto);
    }
}
