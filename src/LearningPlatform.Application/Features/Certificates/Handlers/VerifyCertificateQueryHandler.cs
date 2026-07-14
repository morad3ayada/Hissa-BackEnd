using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Certificates.DTOs;
using LearningPlatform.Application.Features.Certificates.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Certificates.Handlers;

public class VerifyCertificateQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<VerifyCertificateQuery, ApiResponse<CertificateDto>>
{
    public async Task<ApiResponse<CertificateDto>> Handle(VerifyCertificateQuery request, CancellationToken cancellationToken)
    {
        var certificate = await unitOfWork.Repository<Certificate>().AsQueryable()
            .Include(c => c.Student)
            .Include(c => c.Course).ThenInclude(course => course.Instructor)
            .FirstOrDefaultAsync(c => c.CertificateNumber == request.CertificateNumber, cancellationToken)
            ?? throw new NotFoundException("No certificate was found with this certificate number.");

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

        return ApiResponse<CertificateDto>.Success(dto, "This certificate is valid.");
    }
}
