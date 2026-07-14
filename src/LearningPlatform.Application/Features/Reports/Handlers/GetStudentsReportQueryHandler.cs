using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Reports.DTOs;
using LearningPlatform.Application.Features.Reports.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Reports.Handlers;

public class GetStudentsReportQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetStudentsReportQuery, ApiResponse<StudentsReportDto>>
{
    public async Task<ApiResponse<StudentsReportDto>> Handle(GetStudentsReportQuery request, CancellationToken cancellationToken)
    {
        var averageGrade = await unitOfWork.Repository<QuizResult>().AsQueryable()
            .Select(r => (decimal?)r.Score)
            .AverageAsync(cancellationToken) ?? 0;

        var distribution = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .GroupBy(e => new { e.CourseId, e.Course.Title })
            .OrderByDescending(g => g.Count())
            .Select(g => new CourseStudentDistributionDto
            {
                CourseId = g.Key.CourseId,
                CourseTitle = g.Key.Title,
                StudentsCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        var dto = new StudentsReportDto
        {
            AverageGradeAcrossAllQuizzes = Math.Round(averageGrade, 2),
            StudentDistribution = distribution
        };

        return ApiResponse<StudentsReportDto>.Success(dto);
    }
}
