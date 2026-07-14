using LearningPlatform.Application.Features.Authentication.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Commands;

public record RefreshTokenCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string RefreshToken { get; init; } = string.Empty;
}
