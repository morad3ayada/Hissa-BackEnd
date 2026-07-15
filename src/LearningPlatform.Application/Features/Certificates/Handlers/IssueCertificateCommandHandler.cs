using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Certificates.Commands;
using LearningPlatform.Application.Features.Certificates.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Certificates.Handlers;

public class IssueCertificateCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    ICertificatePdfGenerator pdfGenerator,
    IFileStorageService fileStorageService)
    : IRequestHandler<IssueCertificateCommand, ApiResponse<CertificateDto>>
{
    public async Task<ApiResponse<CertificateDto>> Handle(
        IssueCertificateCommand request,
        CancellationToken cancellationToken)
    {
        var course = await unitOfWork.Repository<Course>().GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        if (!currentUser.IsInRole(nameof(UserRole.Admin)) && course.InstructorId != currentUser.UserId)
            throw new ForbiddenException("You are not authorized to issue certificates for this course.");

        var enrollment = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .FirstOrDefaultAsync(
                e => e.StudentId == request.StudentId && e.CourseId == request.CourseId,
                cancellationToken)
            ?? throw new BadRequestException("Student is not enrolled in this course.");

        var alreadyCertified = await unitOfWork.Repository<Certificate>()
            .ExistsAsync(c => c.EnrollmentId == enrollment.Id, cancellationToken);

        if (alreadyCertified)
            throw new BadRequestException("Student already has a certificate for this course.");

        // Mark enrollment as completed if it isn't already
        enrollment.Status = EnrollmentStatus.Completed;
        if (!enrollment.CompletedAt.HasValue)
            enrollment.CompletedAt = DateTime.UtcNow;

        unitOfWork.Repository<Enrollment>().Update(enrollment);

        // Fetch names for the PDF
        var names = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .Where(e => e.Id == enrollment.Id)
            .Select(e => new
            {
                StudentName = e.Student.FirstName + " " + e.Student.LastName,
                CourseTitle = e.Course.Title,
                InstructorName = e.Course.Instructor.FirstName + " " + e.Course.Instructor.LastName
            })
            .FirstAsync(cancellationToken);

        var certificateNumber = $"CERT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        var issuedAt = DateTime.UtcNow;

        var pdfBytes = pdfGenerator.Generate(new CertificatePdfData(
            certificateNumber,
            names.StudentName,
            names.CourseTitle,
            names.InstructorName,
            issuedAt));

        using var pdfStream = new MemoryStream(pdfBytes);
        var certificateUrl = await fileStorageService.UploadAsync(
            pdfStream, $"Certificates/{request.StudentId}/{certificateNumber}.pdf", "application/pdf", cancellationToken);

        var certificate = new Certificate
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrollmentId = enrollment.Id,
            IssuedAt = issuedAt,
            CertificateNumber = certificateNumber,
            CertificateUrl = certificateUrl
        };

        await unitOfWork.Repository<Certificate>().AddAsync(certificate, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new CertificateDto
        {
            Id = certificate.Id,
            CertificateNumber = certificate.CertificateNumber,
            StudentId = certificate.StudentId,
            StudentName = names.StudentName,
            CourseId = certificate.CourseId,
            CourseName = names.CourseTitle,
            InstructorName = names.InstructorName,
            IssuedAt = certificate.IssuedAt
        };

        return ApiResponse<CertificateDto>.Success(dto, "Certificate issued successfully.");
    }
}
