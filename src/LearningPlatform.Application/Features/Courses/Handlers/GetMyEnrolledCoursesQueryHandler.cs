using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Application.Features.Courses.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class GetMyEnrolledCoursesQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetMyEnrolledCoursesQuery, ApiResponse<List<CourseSummaryDto>>>
{
    public async Task<ApiResponse<List<CourseSummaryDto>>> Handle(GetMyEnrolledCoursesQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var courseIds = await unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Where(e => e.StudentId == studentId)
            .Select(e => e.CourseId)
            .ToListAsync(cancellationToken);

        var courses = await unitOfWork.Repository<Course>()
            .AsQueryable()
            .Include(c => c.Instructor)
            .Where(c => courseIds.Contains(c.Id))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<CourseSummaryDto>>.Success(mapper.Map<List<CourseSummaryDto>>(courses));
    }
}