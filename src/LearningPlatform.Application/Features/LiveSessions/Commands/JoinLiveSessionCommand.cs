using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.LiveSessions.Commands;

public record JoinLiveSessionCommand(Guid Id) : IRequest<ApiResponse<LiveSessionDto>>;
