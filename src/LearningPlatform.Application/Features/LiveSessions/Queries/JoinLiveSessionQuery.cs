using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.LiveSessions.Queries;

public record JoinLiveSessionQuery(Guid Id) : IRequest<ApiResponse<LiveSessionDto>>;
