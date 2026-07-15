using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Profiles.Commands;
using LearningPlatform.Application.Features.Profiles.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace LearningPlatform.Application.Features.Profiles.Handlers;

public class UploadProfilePictureCommandHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<UploadProfilePictureCommand, ApiResponse<ProfileDto>>
{
    private const string ImgBbApiKey = "5eb12a56eb82a04bb7a4354d43167947";

    public async Task<ApiResponse<ProfileDto>> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(currentUser.UserId!.Value.ToString())
            ?? throw new NotFoundException("User not found.");

        using var memoryStream = new MemoryStream();
        await request.FileStream.CopyToAsync(memoryStream, cancellationToken);
        var imageBytes = memoryStream.ToArray();
        var base64 = Convert.ToBase64String(imageBytes);

        using var client = new HttpClient();
        var content = new MultipartFormDataContent
        {
            { new StringContent(ImgBbApiKey), "key" },
            { new StringContent(base64), "image" }
        };

        var response = await client.PostAsync("https://api.imgbb.com/1/upload", content, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new BadRequestException("Failed to upload image to ImgBB.");

        using var doc = JsonDocument.Parse(responseBody);
        var imageUrl = doc.RootElement
            .GetProperty("data")
            .GetProperty("url")
            .GetString()!;

        user.ProfilePictureUrl = imageUrl;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));

        var profile = mapper.Map<ProfileDto>(user);
        return ApiResponse<ProfileDto>.Success(profile, "Profile picture updated.");
    }
}