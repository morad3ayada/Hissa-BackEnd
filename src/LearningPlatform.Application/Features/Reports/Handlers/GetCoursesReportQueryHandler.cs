using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Dashboard.DTOs;
using LearningPlatform.Application.Features.Reports.DTOs;
using LearningPlatform.Application.Features.Reports.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Reports.Handlers;

public class GetCoursesReportQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCoursesReportQuery, ApiResponse<CoursesReportDto>>
{
    private const int TopN = 10;

    public async Task<ApiResponse<CoursesReportDto>> Handle(GetCoursesReportQuery request, CancellationToken cancellationToken)
    {
        var topEnrolledCourses = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .GroupBy(e => new { e.CourseId, e.Course.Title })
            .OrderByDescending(g => g.Count())
            .Take(TopN)
            .Select(g => new TopCourseDto { CourseId = g.Key.CourseId, Title = g.Key.Title, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var topViewedCourses = await unitOfWork.Repository<CourseProgress>().AsQueryable()
            .GroupBy(p => new { p.Lesson.CourseSection.CourseId, p.Lesson.CourseSection.Course.Title })
            .OrderByDescending(g => g.Count())
            .Take(TopN)
            .Select(g => new TopCourseDto { CourseId = g.Key.CourseId, Title = g.Key.Title, Count = g.Count() })
            .ToListAsync(cancellationToken);

        // Least active: published courses ranked by fewest enrollments (a draft course would
        // trivially have zero activity, which isn't a meaningful "least active" signal).
        var enrollmentCountsByCourse = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .GroupBy(e => e.CourseId)
            .Select(g => new { CourseId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var publishedCourses = await unitOfWork.Repository<Course>().AsQueryable()
            .Where(c => c.Status == CourseStatus.Published)
            .Select(c => new { c.Id, c.Title })
            .ToListAsync(cancellationToken);

        var leastActiveCourses = publishedCourses
            .Select(c => new TopCourseDto
            {
                CourseId = c.Id,
                Title = c.Title,
                Count = enrollmentCountsByCourse.FirstOrDefault(x => x.CourseId == c.Id)?.Count ?? 0
            })
            .OrderBy(c => c.Count)
            .Take(TopN)
            .ToList();

        var dto = new CoursesReportDto
        {
            TopEnrolledCourses = topEnrolledCourses,
            TopViewedCourses = topViewedCourses,
            LeastActiveCourses = leastActiveCourses
        };

        return ApiResponse<CoursesReportDto>.Success(dto);
    }
}
