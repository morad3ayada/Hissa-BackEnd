using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Commands;

public record LogoutCommand : IRequest<ApiResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
}
