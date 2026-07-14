using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Gamification.Commands;

public record AcceptChallengeCommand(Guid ChallengeId) : IRequest<ApiResponse<ChallengeDto>>;
