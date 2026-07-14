using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Application.Features.Courses.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class GetAllCoursesQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetAllCoursesQuery, PaginatedResponse<CourseSummaryDto>>
{
    public async Task<PaginatedResponse<CourseSummaryDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Course> query = unitOfWork.Repository<Course>().AsQueryable().Include(c => c.Instructor);

        if (currentUser.IsInRole(nameof(UserRole.Admin)))
        {
            // Admins browse the full catalog regardless of status.
        }
        else if (currentUser.IsInRole(nameof(UserRole.Instructor)))
        {
            query = query.Where(c => c.InstructorId == currentUser.UserId);
        }
        else
        {
            query = query.Where(c => c.Status == CourseStatus.Published);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Title.Contains(request.Search));

        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(c => c.Category == request.Category);

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
