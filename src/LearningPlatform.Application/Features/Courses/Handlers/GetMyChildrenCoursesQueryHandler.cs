using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Application.Features.Courses.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class GetMyChildrenCoursesQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetMyChildrenCoursesQuery, ApiResponse<List<CourseSummaryDto>>>
{
    public async Task<ApiResponse<List<CourseSummaryDto>>> Handle(GetMyChildrenCoursesQuery request, CancellationToken cancellationToken)
    {
        var parentId = currentUser.UserId!.Value;

        var childIds = await unitOfWork.Repository<ParentStudent>()
            .AsQueryable()
            .Where(ps => ps.ParentId == parentId)
            .Select(ps => ps.StudentId)
            .ToListAsync(cancellationToken);

        if (childIds.Count == 0)
            return ApiResponse<List<CourseSummaryDto>>.Success(new List<CourseSummaryDto>());

        var courseIds = await unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Where(e => childIds.Contains(e.StudentId))
            .Select(e => e.CourseId)
            .Distinct()
            .ToListAsync(cancellationToken);

        List<Course> courses;
        if (courseIds.Count > 0)
        {
            courses = await unitOfWork.Repository<Course>()
                .AsQueryable()
                .Include(c => c.Instructor)
                .Where(c => courseIds.Contains(c.Id))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        else
        {
            courses = await unitOfWork.Repository<Course>()
                .AsQueryable()
                .Include(c => c.Instructor)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        return ApiResponse<List<CourseSummaryDto>>.Success(mapper.Map<List<CourseSummaryDto>>(courses));
    }
}