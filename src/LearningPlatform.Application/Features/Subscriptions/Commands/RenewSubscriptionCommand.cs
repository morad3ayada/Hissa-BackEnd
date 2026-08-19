using LearningPlatform.Application.Features.Subscriptions.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Subscriptions.Commands;

public record RenewSubscriptionCommand : IRequest<ApiResponse<InstructorSubscriptionDto>>
{
    public string? PaymentReference { get; init; }
}
