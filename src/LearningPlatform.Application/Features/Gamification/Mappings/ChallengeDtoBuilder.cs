using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Gamification.Mappings;

public static class ChallengeDtoBuilder
{
    public static ChallengeDto Build(Challenge challenge, decimal? myScore, decimal? opponentScore) => new()
    {
        Id = challenge.Id,
        Title = challenge.Title,
        Description = challenge.Description,
        ChallengerId = challenge.ChallengerId,
        ChallengerName = $"{challenge.Challenger.FirstName} {challenge.Challenger.LastName}",
        OpponentId = challenge.OpponentId,
        OpponentName = $"{challenge.Opponent.FirstName} {challenge.Opponent.LastName}",
        QuizId = challenge.QuizId,
        QuizTitle = challenge.Quiz.Title,
        DurationInMinutes = challenge.DurationInMinutes,
        Status = challenge.Status.ToString(),
        AcceptedAt = challenge.AcceptedAt,
        Deadline = challenge.AcceptedAt?.AddMinutes(challenge.DurationInMinutes),
        CompletedAt = challenge.CompletedAt,
        PointsReward = challenge.PointsReward,
        WinnerId = challenge.WinnerId,
        WinnerName = challenge.Winner is null ? null : $"{challenge.Winner.FirstName} {challenge.Winner.LastName}",
        MyScore = myScore,
        OpponentScore = opponentScore
    };
}
