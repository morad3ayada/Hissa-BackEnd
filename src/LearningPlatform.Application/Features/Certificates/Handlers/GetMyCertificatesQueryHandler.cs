using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Certificates.DTOs;
using LearningPlatform.Application.Features.Certificates.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Certificates.Handlers;

public class GetMyCertificatesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetMyCertificatesQuery, ApiResponse<List<CertificateDto>>>
{
    public async Task<ApiResponse<List<CertificateDto>>> Handle(GetMyCertificatesQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var certificates = await unitOfWork.Repository<Certificate>().AsQueryable()
            .Include(c => c.Student)
            .Include(c => c.Course).ThenInclude(course => course.Instructor)
            .Where(c => c.StudentId == studentId)
            .OrderByDescending(c => c.IssuedAt)
            .Select(c => new CertificateDto
            {
                Id = c.Id,
                CertificateNumber = c.CertificateNumber,
                StudentId = c.StudentId,
                StudentName = $"{c.Student.FirstName} {c.Student.LastName}",
                CourseId = c.CourseId,
                CourseName = c.Course.Title,
                InstructorName = $"{c.Course.Instructor.FirstName} {c.Course.Instructor.LastName}",
                IssuedAt = c.IssuedAt
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<CertificateDto>>.Success(certificates);
    }
}
