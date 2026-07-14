using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Gamification.Queries;

public record GetMyAchievementsQuery : IRequest<ApiResponse<List<StudentRewardDto>>>;
