using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Progress.DTOs;
using LearningPlatform.Application.Features.Progress.Interfaces;
using LearningPlatform.Application.Features.Progress.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Progress.Handlers;

public class GetStudentProgressQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    ICourseProgressCalculator calculator)
    : IRequestHandler<GetStudentProgressQuery, ApiResponse<List<CourseProgressSummaryDto>>>
{
    public async Task<ApiResponse<List<CourseProgressSummaryDto>>> Handle(GetStudentProgressQuery request, CancellationToken cancellationToken)
    {
        var isAdmin = currentUser.IsInRole(nameof(UserRole.Admin));

        var query = unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Include(e => e.Course)
            .Where(e => e.StudentId == request.StudentId && e.Status == EnrollmentStatus.Active);

        // Instructors only see this student's progress within courses the instructor owns.
        if (!isAdmin)
        {
            var instructorId = currentUser.UserId!.Value;
            query = query.Where(e => e.Course.InstructorId == instructorId);
        }

        var courseIds = await query.Select(e => e.CourseId).ToListAsync(cancellationToken);

        var summaries = new List<CourseProgressSummaryDto>();
        foreach (var courseId in courseIds)
            summaries.Add(await calculator.CalculateSummaryAsync(courseId, request.StudentId, cancellationToken));

        return ApiResponse<List<CourseProgressSummaryDto>>.Success(summaries);
    }
}
