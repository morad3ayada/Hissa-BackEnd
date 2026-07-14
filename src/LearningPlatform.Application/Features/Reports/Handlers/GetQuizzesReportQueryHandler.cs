using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Reports.DTOs;
using LearningPlatform.Application.Features.Reports.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Reports.Handlers;

public class GetQuizzesReportQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetQuizzesReportQuery, ApiResponse<QuizzesReportDto>>
{
    private const int TopN = 10;

    public async Task<ApiResponse<QuizzesReportDto>> Handle(GetQuizzesReportQuery request, CancellationToken cancellationToken)
    {
        var perQuizStats = await unitOfWork.Repository<QuizResult>().AsQueryable()
            .GroupBy(r => new { r.QuizId, r.Quiz.Title })
            .Select(g => new
            {
                g.Key.QuizId,
                g.Key.Title,
                TotalAttempts = g.Count(),
                PassedCount = g.Count(r => r.IsPassed)
            })
            .ToListAsync(cancellationToken);

        var mostDifficultQuizzes = perQuizStats
            .Where(q => q.TotalAttempts > 0)
            .Select(q => new DifficultQuizDto
            {
                QuizId = q.QuizId,
                Title = q.Title,
                TotalAttempts = q.TotalAttempts,
                PassedCount = q.PassedCount,
                PassRate = Math.Round(q.PassedCount * 100m / q.TotalAttempts, 2)
            })
            .OrderBy(q => q.PassRate)
            .Take(TopN)
            .ToList();

        var totalAttempts = perQuizStats.Sum(q => q.TotalAttempts);
        var passedCount = perQuizStats.Sum(q => q.PassedCount);
        var failedCount = totalAttempts - passedCount;

        var dto = new QuizzesReportDto
        {
            MostDifficultQuizzes = mostDifficultQuizzes,
            TotalAttempts = totalAttempts,
            PassedCount = passedCount,
            FailedCount = failedCount,
            PassRate = totalAttempts == 0 ? 0 : Math.Round(passedCount * 100m / totalAttempts, 2),
            FailRate = totalAttempts == 0 ? 0 : Math.Round(failedCount * 100m / totalAttempts, 2)
        };

        return ApiResponse<QuizzesReportDto>.Success(dto);
    }
}
