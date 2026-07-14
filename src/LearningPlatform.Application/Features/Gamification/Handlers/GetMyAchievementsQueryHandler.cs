using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Application.Features.Gamification.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Gamification.Handlers;

public class GetMyAchievementsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetMyAchievementsQuery, ApiResponse<List<StudentRewardDto>>>
{
    public async Task<ApiResponse<List<StudentRewardDto>>> Handle(GetMyAchievementsQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var achievements = await unitOfWork.Repository<StudentReward>().AsQueryable()
            .Include(sr => sr.Reward)
            .Where(sr => sr.StudentId == studentId && sr.Reward.TriggerType != null)
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
                SourceChallengeTitle = null
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<StudentRewardDto>>.Success(achievements);
    }
}
