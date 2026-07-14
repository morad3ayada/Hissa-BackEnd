using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Gamification.Queries;

public record GetLeaderboardQuery : IRequest<PaginatedResponse<LeaderboardEntryDto>>
{
    public Guid? CourseId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
