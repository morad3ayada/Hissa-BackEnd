using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Application.Features.Gamification.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Gamification.Handlers;

public class GetAvatarStoreQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetAvatarStoreQuery, ApiResponse<List<AvatarItemDto>>>
{
    public async Task<ApiResponse<List<AvatarItemDto>>> Handle(GetAvatarStoreQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var items = await unitOfWork.Repository<AvatarItem>().GetAllAsync(cancellationToken);

        var owned = (await unitOfWork.Repository<StudentAvatar>()
            .FindAsync(sa => sa.StudentId == studentId, cancellationToken))
            .ToDictionary(sa => sa.AvatarItemId, sa => sa.IsEquipped);

        var dtos = items
            .OrderBy(i => i.Category)
            .ThenBy(i => i.Name)
            .Select(i => new AvatarItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Category = i.Category,
                ImageUrl = i.ImageUrl,
                PriceInPoints = i.PriceInPoints,
                IsDefault = i.IsDefault,
                Owned = owned.ContainsKey(i.Id),
                Equipped = owned.TryGetValue(i.Id, out var isEquipped) && isEquipped
            })
            .ToList();

        return ApiResponse<List<AvatarItemDto>>.Success(dtos);
    }
}
