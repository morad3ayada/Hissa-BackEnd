using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Application.Features.Gamification.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Gamification.Handlers;

public class GetProfileQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetProfileQuery, ApiResponse<GamificationProfileDto>>
{
    public async Task<ApiResponse<GamificationProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var profile = (await unitOfWork.Repository<GamificationProfile>()
            .FindAsync(p => p.StudentId == studentId, cancellationToken)).FirstOrDefault();

        var totalPoints = profile?.TotalPoints ?? 0;
        var currentLevel = profile?.CurrentLevel ?? 1;
        var avatarGender = profile?.AvatarGender ?? AvatarGender.Boy;

        var levels = await unitOfWork.Repository<GamificationLevel>().GetAllAsync(cancellationToken);
        var levelsByNumber = levels.ToDictionary(l => l.LevelNumber);

        var levelTitle = levelsByNumber.TryGetValue(currentLevel, out var currentLevelRow) ? currentLevelRow.Title : "Beginner";
        var pointsToNextLevel = levelsByNumber.TryGetValue(currentLevel + 1, out var nextLevelRow)
            ? nextLevelRow.RequiredPoints - totalPoints
            : (int?)null;

        var rank = await unitOfWork.Repository<GamificationProfile>().AsQueryable()
            .CountAsync(p => p.TotalPoints > totalPoints, cancellationToken) + 1;

        var equippedItems = await unitOfWork.Repository<StudentAvatar>().AsQueryable()
            .Include(sa => sa.AvatarItem)
            .Where(sa => sa.StudentId == studentId && sa.IsEquipped)
            .Select(sa => new AvatarItemDto
            {
                Id = sa.AvatarItem.Id,
                Name = sa.AvatarItem.Name,
                Category = sa.AvatarItem.Category,
                ImageUrl = sa.AvatarItem.ImageUrl,
                PriceInPoints = sa.AvatarItem.PriceInPoints,
                IsDefault = sa.AvatarItem.IsDefault,
                Owned = true,
                Equipped = true
            })
            .ToListAsync(cancellationToken);

        var dto = new GamificationProfileDto
        {
            TotalPoints = totalPoints,
            CurrentLevel = currentLevel,
            LevelTitle = levelTitle,
            PointsToNextLevel = pointsToNextLevel,
            Rank = rank,
            AvatarGender = avatarGender.ToString(),
            EquippedItems = equippedItems
        };

        return ApiResponse<GamificationProfileDto>.Success(dto);
    }
}
