using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Application.Features.Quizzes.Commands;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Gamification.Commands;

public record SubmitChallengeCommand(Guid ChallengeId, List<SubmitAnswerInput> Answers) : IRequest<ApiResponse<ChallengeDto>>;
