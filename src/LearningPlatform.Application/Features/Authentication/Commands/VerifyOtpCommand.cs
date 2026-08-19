using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Commands;

public record VerifyOtpCommand : IRequest<ApiResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Otp { get; init; } = string.Empty;
}
