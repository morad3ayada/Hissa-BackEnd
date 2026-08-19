using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Application.Features.Chat.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class GetBlockedUsersQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<GetBlockedUsersQuery, PaginatedResponse<BlockedUserDto>>
{
    public async Task<PaginatedResponse<BlockedUserDto>> Handle(GetBlockedUsersQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var query = unitOfWork.Repository<BlockedUser>().AsQueryable()
            .Where(b => b.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var blocked = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(b => b.BlockedUserInfo)
            .ToListAsync(cancellationToken);

        var items = blocked
            .Select(b => new BlockedUserDto
            {
                UserId = b.BlockedUserId,
                FirstName = b.BlockedUserInfo.FirstName,
                LastName = b.BlockedUserInfo.LastName,
                ProfilePictureUrl = b.BlockedUserInfo.ProfilePictureUrl,
                BlockedAt = b.CreatedAt
            })
            .ToList();

        return PaginatedResponse<BlockedUserDto>.Create(
            new PaginatedList<BlockedUserDto>(items, totalCount, request.PageNumber, request.PageSize));
    }
}
