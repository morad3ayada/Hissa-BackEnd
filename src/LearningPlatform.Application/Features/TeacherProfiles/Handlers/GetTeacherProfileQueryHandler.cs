using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.TeacherProfiles.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.TeacherProfiles.Handlers;

public class GetTeacherProfileQueryHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Queries.GetTeacherProfileQuery, ApiResponse<TeacherProfileDto>>
{
    public async Task<ApiResponse<TeacherProfileDto>> Handle(
        Queries.GetTeacherProfileQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found.");

        if (user.Role != UserRole.Instructor)
            throw new ForbiddenException("Only instructors can view teacher profiles.");

        var profile = await unitOfWork.Repository<TeacherProfile>()
            .GetTrackedAsync(t => t.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Teacher profile not found. Please create your profile first.");

        var dto = new TeacherProfileDto
        {
            Id = profile.Id,
            ProfileImageUrl = profile.ProfileImageUrl,
            RealName = profile.RealName,
            Specialization = profile.Specialization,
            Subjects = profile.Subjects,
            Grades = profile.Grades,
            Governorate = profile.Governorate,
            YearsOfExperience = profile.YearsOfExperience,
            Bio = profile.Bio,
            LessonPrice = profile.LessonPrice,
            Certificates = profile.Certificates,
            Qualifications = profile.Qualifications,
            RequiredDocuments = profile.RequiredDocuments,
            VerificationStatus = profile.VerificationStatus,
            RejectionReason = profile.RejectionReason,
            AcceptingBookings = profile.AcceptingBookings,
            CanReceiveBookings = profile.CanReceiveBookings,
            UserId = profile.UserId,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };

        return ApiResponse<TeacherProfileDto>.Success(dto);
    }
}
