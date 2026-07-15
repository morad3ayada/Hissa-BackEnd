using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Application.Features.Parents.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Parents.Handlers;

public class GetChildEnrollmentsQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetChildEnrollmentsQuery, ApiResponse<List<EnrollmentDto>>>
{
    public async Task<ApiResponse<List<EnrollmentDto>>> Handle(GetChildEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var parentId = currentUser.UserId!.Value;

        var isLinked = await unitOfWork.Repository<ParentStudent>()
            .ExistsAsync(ps => ps.ParentId == parentId && ps.StudentId == request.StudentId, cancellationToken);

        if (!isLinked)
            throw new ForbiddenException("This student is not linked to you.");

        var enrollments = await unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Include(e => e.Course)
            .Where(e => e.StudentId == request.StudentId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync(cancellationToken);

        var dtos = mapper.Map<List<EnrollmentDto>>(enrollments);
        return ApiResponse<List<EnrollmentDto>>.Success(dtos);
    }
}
