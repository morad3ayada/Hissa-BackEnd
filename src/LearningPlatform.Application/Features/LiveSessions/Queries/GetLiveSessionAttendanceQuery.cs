using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.LiveSessions.Queries;

public record GetLiveSessionAttendanceQuery(Guid Id) : IRequest<ApiResponse<List<LiveSessionAttendanceDto>>>;