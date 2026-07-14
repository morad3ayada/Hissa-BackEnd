using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Application.Features.Gamification.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Gamification.Handlers;

public class GetMyRewardsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetMyRewardsQuery, ApiResponse<List<StudentRewardDto>>>
{
    public async Task<ApiResponse<List<StudentRewardDto>>> Handle(GetMyRewardsQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var rewards = await unitOfWork.Repository<StudentReward>().AsQueryable()
            .Include(sr => sr.Reward)
            .Include(sr => sr.SourceChallenge)
            .Where(sr => sr.StudentId == studentId)
            .OrderByDescending(sr => sr.EarnedAt)
            .Select(sr => new StudentRewardDto
            {
                Id = sr.Id,
                RewardId = sr.RewardId,
                RewardName = sr.Reward.Name,
                RewardDescription = sr.Reward.Description,
                RewardType = sr.Reward.Type.ToString(),
                PointsValue = sr.Reward.PointsValue,
                IconUrl = sr.Reward.IconUrl,
                TriggerType = sr.Reward.TriggerType == null ? null : sr.Reward.TriggerType.ToString(),
                EarnedAt = sr.EarnedAt,
                SourceChallengeId = sr.SourceChallengeId,
                SourceChallengeTitle = sr.SourceChallenge != null ? sr.SourceChallenge.Title : null
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<StudentRewardDto>>.Success(rewards);
    }
}
