using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Application.Features.Courses.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class GetCoursesForAdminQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetCoursesForAdminQuery, ApiResponse<List<CourseSummaryDto>>>
{
    public async Task<ApiResponse<List<CourseSummaryDto>>> Handle(GetCoursesForAdminQuery request, CancellationToken cancellationToken)
    {
        var items = await unitOfWork.Repository<Course>()
            .AsQueryable()
            .Include(c => c.Instructor)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<CourseSummaryDto>>.Success(mapper.Map<List<CourseSummaryDto>>(items));
    }
}