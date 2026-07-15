using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Application.Features.Enrollments.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Enrollments.Handlers;

public class GetStudentEnrollmentsQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetStudentEnrollmentsQuery, ApiResponse<List<EnrollmentDto>>>
{
    public async Task<ApiResponse<List<EnrollmentDto>>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        if (!currentUser.IsInRole(nameof(UserRole.Admin)) && userId != request.StudentId)
        {
            var isLinked = await unitOfWork.Repository<ParentStudent>()
                .ExistsAsync(ps => ps.ParentId == userId && ps.StudentId == request.StudentId, cancellationToken);

            if (!isLinked)
                throw new ForbiddenException("You are not authorized to view this student's enrollments.");
        }

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