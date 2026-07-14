using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Application.Features.Courses.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class GetCoursesForAdminQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetCoursesForAdminQuery, PaginatedResponse<CourseSummaryDto>>
{
    public async Task<PaginatedResponse<CourseSummaryDto>> Handle(GetCoursesForAdminQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Course> query = unitOfWork.Repository<Course>().AsQueryable().Include(c => c.Instructor);

        if (request.Status.HasValue)
            query = query.Where(c => c.Status == request.Status.Value);

        if (request.InstructorId.HasValue)
            query = query.Where(c => c.InstructorId == request.InstructorId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Title.Contains(request.Search));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = mapper.Map<List<CourseSummaryDto>>(items);
        var paginatedList = new PaginatedList<CourseSummaryDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        return PaginatedResponse<CourseSummaryDto>.Create(paginatedList);
    }
}
