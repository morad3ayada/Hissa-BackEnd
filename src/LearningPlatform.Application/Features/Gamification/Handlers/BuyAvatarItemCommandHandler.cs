using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Gamification.Commands;
using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Gamification.Handlers;

public class BuyAvatarItemCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<BuyAvatarItemCommand, ApiResponse<AvatarItemDto>>
{
    private const string BaseCategory = "Base";

    public async Task<ApiResponse<AvatarItemDto>> Handle(BuyAvatarItemCommand request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var item = await unitOfWork.Repository<AvatarItem>().GetByIdAsync(request.AvatarItemId, cancellationToken)
            ?? throw new NotFoundException(nameof(AvatarItem), request.AvatarItemId);

        var alreadyOwned = await unitOfWork.Repository<StudentAvatar>().ExistsAsync(
            sa => sa.StudentId == studentId && sa.AvatarItemId == item.Id, cancellationToken);

        if (alreadyOwned)
            throw new ConflictException("You already own this item.");

        // Single fetch-or-create profile touch (points deduction and/or Base-category gender
        // sync), with exactly one Add/Update call below — never re-fetch within this handler.
        var profile = await unitOfWork.Repository<GamificationProfile>()
            .GetTrackedAsync(p => p.StudentId == studentId, cancellationToken);
        var isNewProfile = false;

        if (item.PriceInPoints > 0)
        {
            if (profile is null || profile.TotalPoints < item.PriceInPoints)
                throw new BadRequestException("You don't have enough points to purchase this item.");

            profile.TotalPoints -= item.PriceInPoints;
        }

        if (item.Category == BaseCategory)
        {
            var gender = item.Name.Contains("Girl", StringComparison.OrdinalIgnoreCase) ? AvatarGender.Girl : AvatarGender.Boy;

            if (profile is null)
            {
                profile = new GamificationProfile { StudentId = studentId, TotalPoints = 0, CurrentLevel = 1, AvatarGender = gender };
                isNewProfile = true;
            }
            else
            {
                profile.AvatarGender = gender;
            }
        }

        if (profile is not null)
        {
            if (isNewProfile)
                await unitOfWork.Repository<GamificationProfile>().AddAsync(profile, cancellationToken);
            else
                unitOfWork.Repository<GamificationProfile>().Update(profile);
        }

        // Only one equipped item per category — buying (or freely claiming) a new one swaps out
        // whatever was equipped in that category before.
        var equippedSiblings = await unitOfWork.Repository<StudentAvatar>().AsQueryable()
            .Include(sa => sa.AvatarItem)
            .Where(sa => sa.StudentId == studentId && sa.IsEquipped && sa.AvatarItem.Category == item.Category)
            .ToListAsync(cancellationToken);

        foreach (var sibling in equippedSiblings)
        {
            sibling.IsEquipped = false;
            unitOfWork.Repository<StudentAvatar>().Update(sibling);
        }

        var purchase = new StudentAvatar
        {
            StudentId = studentId,
            AvatarItemId = item.Id,
            IsEquipped = true,
            AcquiredAt = DateTime.UtcNow
        };

        await unitOfWork.Repository<StudentAvatar>().AddAsync(purchase, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new AvatarItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Category = item.Category,
            ImageUrl = item.ImageUrl,
            PriceInPoints = item.PriceInPoints,
            IsDefault = item.IsDefault,
            Owned = true,
            Equipped = true
        };

        return ApiResponse<AvatarItemDto>.Success(dto, "Item purchased and equipped.");
    }
}
