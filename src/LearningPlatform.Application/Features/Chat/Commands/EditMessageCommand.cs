using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Commands;

public record EditMessageCommand(Guid MessageId, string Content) : IRequest<ApiResponse<MessageDto>>;
