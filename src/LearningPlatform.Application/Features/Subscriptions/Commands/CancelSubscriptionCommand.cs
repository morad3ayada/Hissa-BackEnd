using LearningPlatform.Application.Features.Subscriptions.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Subscriptions.Commands;

public record CancelSubscriptionCommand : IRequest<ApiResponse>;
