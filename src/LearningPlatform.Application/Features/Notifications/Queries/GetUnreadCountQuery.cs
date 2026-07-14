using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Notifications.Queries;

public record GetUnreadCountQuery : IRequest<ApiResponse<int>>;
