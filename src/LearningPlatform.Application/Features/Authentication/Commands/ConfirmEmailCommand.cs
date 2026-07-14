using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Commands;

public record ConfirmEmailCommand : IRequest<ApiResponse>
{
    public Guid UserId { get; init; }
    public string Token { get; init; } = string.Empty;
}
