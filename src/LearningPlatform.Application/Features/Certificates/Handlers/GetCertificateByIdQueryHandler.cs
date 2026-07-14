using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Certificates.DTOs;
using LearningPlatform.Application.Features.Certificates.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Certificates.Handlers;

public class GetCertificateByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetCertificateByIdQuery, ApiResponse<CertificateDto>>
{
    public async Task<ApiResponse<CertificateDto>> Handle(GetCertificateByIdQuery request, CancellationToken cancellationToken)
    {
        var certificate = await unitOfWork.Repository<Certificate>().AsQueryable()
            .Include(c => c.Student)
            .Include(c => c.Course).ThenInclude(course => course.Instructor)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Certificate), request.Id);

        var isOwner = currentUser.UserId == certificate.StudentId;
        var isAdmin = currentUser.IsInRole(nameof(UserRole.Admin));

        if (!isOwner && !isAdmin)
            throw new ForbiddenException("You do not have permission to view this certificate.");

        var dto = new CertificateDto
        {
            Id = certificate.Id,
            CertificateNumber = certificate.CertificateNumber,
            StudentId = certificate.StudentId,
            StudentName = $"{certificate.Student.FirstName} {certificate.Student.LastName}",
            CourseId = certificate.CourseId,
            CourseName = certificate.Course.Title,
            InstructorName = $"{certificate.Course.Instructor.FirstName} {certificate.Course.Instructor.LastName}",
            IssuedAt = certificate.IssuedAt
        };

        return ApiResponse<CertificateDto>.Success(dto);
    }
}
