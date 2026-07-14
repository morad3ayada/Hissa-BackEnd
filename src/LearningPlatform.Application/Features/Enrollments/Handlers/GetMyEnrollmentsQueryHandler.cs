using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Application.Features.Enrollments.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Enrollments.Handlers;

public class GetMyEnrollmentsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    : IRequestHandler<GetMyEnrollmentsQuery, PaginatedResponse<EnrollmentDto>>
{
    public async Task<PaginatedResponse<EnrollmentDto>> Handle(GetMyEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var query = unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Include(e => e.Course)
            .Include(e => e.Student)
            .Where(e => e.StudentId == studentId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = mapper.Map<List<EnrollmentDto>>(items);
        var paginatedList = new PaginatedList<EnrollmentDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        return PaginatedResponse<EnrollmentDto>.Create(paginatedList);
    }
}
