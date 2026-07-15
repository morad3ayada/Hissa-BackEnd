using LearningPlatform.Application.Features.Profiles.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Profiles.Commands;

public class UploadProfilePictureCommand : IRequest<ApiResponse<ProfileDto>>
{
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}