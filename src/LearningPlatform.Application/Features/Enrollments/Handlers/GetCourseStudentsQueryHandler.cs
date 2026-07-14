using AutoMapper;
using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Application.Features.Enrollments.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Enrollments.Handlers;

public class GetCourseStudentsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    : IRequestHandler<GetCourseStudentsQuery, PaginatedResponse<CourseStudentDto>>
{
    public async Task<PaginatedResponse<CourseStudentDto>> Handle(GetCourseStudentsQuery request, CancellationToken cancellationToken)
    {
        var course = await unitOfWork.Repository<Course>().GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        currentUser.EnsureCanManageCourse(course);

        var query = unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Include(e => e.Student)
            .Where(e => e.CourseId == request.CourseId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = mapper.Map<List<CourseStudentDto>>(items);
        var paginatedList = new PaginatedList<CourseStudentDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        return PaginatedResponse<CourseStudentDto>.Create(paginatedList);
    }
}
