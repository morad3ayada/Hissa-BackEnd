using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Progress.DTOs;
using LearningPlatform.Application.Features.Progress.Interfaces;
using LearningPlatform.Application.Features.Progress.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Progress.Handlers;

public class GetMyCoursesProgressQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    ICourseProgressCalculator calculator)
    : IRequestHandler<GetMyCoursesProgressQuery, PaginatedResponse<CourseProgressSummaryDto>>
{
    public async Task<PaginatedResponse<CourseProgressSummaryDto>> Handle(GetMyCoursesProgressQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var query = unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Active);

        var totalCount = await query.CountAsync(cancellationToken);

        var courseIds = await query
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => e.CourseId)
            .ToListAsync(cancellationToken);

        var summaries = new List<CourseProgressSummaryDto>();
        foreach (var courseId in courseIds)
            summaries.Add(await calculator.CalculateSummaryAsync(courseId, studentId, cancellationToken));

        var paginatedList = new PaginatedList<CourseProgressSummaryDto>(summaries, totalCount, request.PageNumber, request.PageSize);

        return PaginatedResponse<CourseProgressSummaryDto>.Create(paginatedList);
    }
}
