using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Application.Features.Courses.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class GetAllCoursesQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetAllCoursesQuery, ApiResponse<List<CourseSummaryDto>>>
{
    public async Task<ApiResponse<List<CourseSummaryDto>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Course> query = unitOfWork.Repository<Course>().AsQueryable().Include(c => c.Instructor);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = mapper.Map<List<CourseSummaryDto>>(items);
        return ApiResponse<List<CourseSummaryDto>>.Success(dtos);
    }
}