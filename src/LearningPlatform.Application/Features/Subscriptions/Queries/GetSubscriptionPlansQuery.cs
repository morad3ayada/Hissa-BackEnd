using LearningPlatform.Application.Features.Subscriptions.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Subscriptions.Queries;

public record GetSubscriptionPlansQuery : IRequest<ApiResponse<List<SubscriptionPlanDto>>>;
