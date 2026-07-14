using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Gamification.Queries;

public record GetAvatarStoreQuery : IRequest<ApiResponse<List<AvatarItemDto>>>;
