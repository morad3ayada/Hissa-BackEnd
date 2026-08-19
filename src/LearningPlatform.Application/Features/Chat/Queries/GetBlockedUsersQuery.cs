using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Queries;

public record GetBlockedUsersQuery : IRequest<PaginatedResponse<BlockedUserDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
