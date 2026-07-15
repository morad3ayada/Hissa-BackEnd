using LearningPlatform.Application.Features.Profiles.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Profiles.Commands;

public record UpdateMyProfileCommand : IRequest<ApiResponse<ProfileDto>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Bio { get; init; }
}
