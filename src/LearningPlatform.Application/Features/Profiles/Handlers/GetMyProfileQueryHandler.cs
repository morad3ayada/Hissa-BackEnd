using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Profiles.DTOs;
using LearningPlatform.Application.Features.Profiles.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Profiles.Handlers;

public class GetMyProfileQueryHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetMyProfileQuery, ApiResponse<ProfileDto>>
{
    public async Task<ApiResponse<ProfileDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(currentUser.UserId!.Value.ToString())
            ?? throw new NotFoundException("User not found.");

        var profile = mapper.Map<ProfileDto>(user);
        return ApiResponse<ProfileDto>.Success(profile);
    }
}
