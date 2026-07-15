using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Profiles.Commands;
using LearningPlatform.Application.Features.Profiles.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Profiles.Handlers;

public class UpdateMyProfileCommandHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<UpdateMyProfileCommand, ApiResponse<ProfileDto>>
{
    public async Task<ApiResponse<ProfileDto>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(currentUser.UserId!.Value.ToString())
            ?? throw new NotFoundException("User not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Bio = request.Bio;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));

        var profile = mapper.Map<ProfileDto>(user);
        return ApiResponse<ProfileDto>.Success(profile, "Profile updated successfully.");
    }
}
