using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Application.Features.Enrollments.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Enrollments.Handlers;

public class GetEnrollmentQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    : IRequestHandler<GetEnrollmentQuery, ApiResponse<EnrollmentDto>>
{
    public async Task<ApiResponse<EnrollmentDto>> Handle(GetEnrollmentQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var enrollment = await unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Include(e => e.Course)
            .Include(e => e.Student)
            .FirstOrDefaultAsync(e => e.CourseId == request.CourseId && e.StudentId == studentId, cancellationToken)
            ?? throw new NotFoundException("You are not enrolled in this course.");

        return ApiResponse<EnrollmentDto>.Success(mapper.Map<EnrollmentDto>(enrollment));
    }
}
