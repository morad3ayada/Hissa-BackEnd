using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.LiveSessions.Commands;

public record DeleteLiveSessionCommand(Guid Id) : IRequest<ApiResponse>;
