using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.TeacherProfiles.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.TeacherProfiles.Handlers;

public class GetPendingTeachersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<Queries.GetPendingTeachersQuery, ApiResponse<List<PendingTeacherDto>>>
{
    public async Task<ApiResponse<List<PendingTeacherDto>>> Handle(
        Queries.GetPendingTeachersQuery request, CancellationToken cancellationToken)
    {
        var teachers = await unitOfWork.Repository<TeacherProfile>()
            .AsQueryable()
            .Where(t => t.VerificationStatus == TeacherVerificationStatus.UnderReview)
            .OrderBy(t => t.CreatedAt)
            .Select(t => new PendingTeacherDto
            {
                TeacherProfileId = t.Id,
                UserId = t.UserId,
                RealName = t.RealName,
                Specialization = t.Specialization,
                Governorate = t.Governorate,
                YearsOfExperience = t.YearsOfExperience,
                LessonPrice = t.LessonPrice,
                Subjects = t.Subjects,
                Grades = t.Grades,
                Certificates = t.Certificates,
                Qualifications = t.Qualifications,
                RequiredDocuments = t.RequiredDocuments,
                VerificationStatus = t.VerificationStatus,
                RejectionReason = t.RejectionReason,
                SubmittedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<PendingTeacherDto>>.Success(teachers);
    }
}
