using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Queries;

public record GetUnreadCountQuery : IRequest<ApiResponse<UnreadCountDto>>;
