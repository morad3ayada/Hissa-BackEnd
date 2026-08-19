using LearningPlatform.Application.Features.Subscriptions.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Subscriptions.Commands;

public record SubscribeCommand : IRequest<ApiResponse<InstructorSubscriptionDto>>
{
    public Guid PlanId { get; init; }
    public string? PaymentReference { get; init; }
}
