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

public class GetParentDashboardQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, ICourseProgressCalculator courseProgressCalculator)
    : IRequestHandler<GetParentDashboardQuery, ApiResponse<ParentDashboardDto>>
{
    private const int RecentQuizzesCount = 10;

    public async Task<ApiResponse<ParentDashboardDto>> Handle(GetParentDashboardQuery request, CancellationToken cancellationToken)
    {
        var parentId = currentUser.UserId!.Value;

        var children = await unitOfWork.Repository<ParentStudent>().AsQueryable()
            .Include(ps => ps.Student)
            .Where(ps => ps.ParentId == parentId)
            .Select(ps => new { ps.StudentId, StudentName = ps.Student.FirstName + " " + ps.Student.LastName })
            .ToListAsync(cancellationToken);

        var result = new List<ChildDashboardDto>();

        foreach (var child in children)
        {
            var enrolledCourseIds = await unitOfWork.Repository<Enrollment>().AsQueryable()
                .Where(e => e.StudentId == child.StudentId && e.Status == EnrollmentStatus.Active)
                .Select(e => e.CourseId)
                .ToListAsync(cancellationToken);

            var overallProgress = 0m;
            if (enrolledCourseIds.Count > 0)
            {
                var percentages = new List<decimal>();
                foreach (var courseId in enrolledCourseIds)
                {
                    var summary = await courseProgressCalculator.CalculateSummaryAsync(courseId, child.StudentId, cancellationToken);
                    percentages.Add(summary.CompletionPercentage);
                }

                overallProgress = Math.Round(percentages.Average(), 2);
            }

            var quizScores = await unitOfWork.Repository<QuizResult>().AsQueryable()
                .Include(r => r.Quiz)
                .Where(r => r.StudentId == child.StudentId)
                .OrderByDescending(r => r.CompletedAt)
                .Take(RecentQuizzesCount)
                .Select(r => new QuizScoreDto
                {
                    QuizTitle = r.Quiz.Title,
                    Score = r.Score,
                    IsPassed = r.IsPassed,
                    CompletedAt = r.CompletedAt ?? r.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var errorsCount = await unitOfWork.Repository<ErrorBank>().AsQueryable()
                .CountAsync(e => e.StudentId == child.StudentId && !e.IsResolved, cancellationToken);

            var totalWatchedSeconds = await unitOfWork.Repository<CourseProgress>().AsQueryable()
                .Where(p => p.StudentId == child.StudentId)
                .SumAsync(p => (double?)p.CurrentSecond, cancellationToken) ?? 0;

            var lastActivityAt = await unitOfWork.Repository<CourseProgress>().AsQueryable()
                .Where(p => p.StudentId == child.StudentId)
                .OrderByDescending(p => p.LastWatchedAt)
                .Select(p => p.LastWatchedAt)
                .FirstOrDefaultAsync(cancellationToken);

            result.Add(new ChildDashboardDto
            {
                StudentId = child.StudentId,
                StudentName = child.StudentName,
                OverallProgressPercentage = overallProgress,
                QuizScores = quizScores,
                ErrorsCount = errorsCount,
                StudyHours = Math.Round(totalWatchedSeconds / 3600.0, 2),
                LastActivityAt = lastActivityAt
            });
        }

        return ApiResponse<ParentDashboardDto>.Success(new ParentDashboardDto { Children = result });
    }
}
