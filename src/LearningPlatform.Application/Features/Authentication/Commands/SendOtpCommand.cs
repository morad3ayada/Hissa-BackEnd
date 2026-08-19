using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Commands;

public record SendOtpCommand : IRequest<ApiResponse>
{
    public string Email { get; init; } = string.Empty;
}
