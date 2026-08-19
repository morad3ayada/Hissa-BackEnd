using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.TeacherProfiles.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.TeacherProfiles.Handlers;

public class UpdateTeacherProfileCommandHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Commands.UpdateTeacherProfileCommand, ApiResponse<TeacherProfileDto>>
{
    public async Task<ApiResponse<TeacherProfileDto>> Handle(
        Commands.UpdateTeacherProfileCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found.");

        if (user.Role != UserRole.Instructor)
            throw new ForbiddenException("Only instructors can update teacher profiles.");

        var existingProfile = await unitOfWork.Repository<TeacherProfile>()
            .GetTrackedAsync(t => t.UserId == userId, cancellationToken);

        if (existingProfile is null)
        {
            var profile = new TeacherProfile
            {
                UserId = userId,
                ProfileImageUrl = request.ProfileImageUrl,
                RealName = request.RealName,
                Specialization = request.Specialization,
                Subjects = request.Subjects,
                Grades = request.Grades,
                Governorate = request.Governorate,
                YearsOfExperience = request.YearsOfExperience,
                Bio = request.Bio,
                LessonPrice = request.LessonPrice,
                Certificates = request.Certificates,
                Qualifications = request.Qualifications,
                RequiredDocuments = request.RequiredDocuments
            };

            await unitOfWork.Repository<TeacherProfile>().AddAsync(profile, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<TeacherProfileDto>.Success(
                MapToDto(profile),
                "Teacher profile created successfully.");
        }

        existingProfile.ProfileImageUrl = request.ProfileImageUrl;
        existingProfile.RealName = request.RealName;
        existingProfile.Specialization = request.Specialization;
        existingProfile.Subjects = request.Subjects;
        existingProfile.Grades = request.Grades;
        existingProfile.Governorate = request.Governorate;
        existingProfile.YearsOfExperience = request.YearsOfExperience;
        existingProfile.Bio = request.Bio;
        existingProfile.LessonPrice = request.LessonPrice;
        existingProfile.Certificates = request.Certificates;
        existingProfile.Qualifications = request.Qualifications;
        existingProfile.RequiredDocuments = request.RequiredDocuments;

        unitOfWork.Repository<TeacherProfile>().Update(existingProfile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<TeacherProfileDto>.Success(
            MapToDto(existingProfile),
            "Teacher profile updated successfully.");
    }

    private static TeacherProfileDto MapToDto(TeacherProfile profile) => new()
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
}
