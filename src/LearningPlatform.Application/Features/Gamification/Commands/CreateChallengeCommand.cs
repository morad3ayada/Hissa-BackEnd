using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Gamification.Commands;

public record CreateChallengeCommand : IRequest<ApiResponse<ChallengeDto>>
{
    public Guid? ChallengerId { get; init; }
    public Guid OpponentId { get; init; }
    public Guid QuizId { get; init; }
    public int DurationInMinutes { get; init; }
    public string? Title { get; init; }
}
