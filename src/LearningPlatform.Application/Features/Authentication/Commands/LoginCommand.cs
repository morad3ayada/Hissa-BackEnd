using LearningPlatform.Application.Features.Authentication.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Commands;

public record LoginCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
