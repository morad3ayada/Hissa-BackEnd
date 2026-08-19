using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Application.Features.Chat.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class EditMessageCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IChatQueryService chatQueryService)
    : IRequestHandler<EditMessageCommand, ApiResponse<MessageDto>>
{
    public async Task<ApiResponse<MessageDto>> Handle(EditMessageCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var message = await unitOfWork.Repository<ChatMessage>()
            .GetTrackedAsync(m => m.Id == request.MessageId, cancellationToken)
            ?? throw new NotFoundException(nameof(ChatMessage), request.MessageId);

        if (message.SenderId != userId)
            throw new ForbiddenException("You can only edit your own messages.");

        if (message.MessageType != ChatMessageType.Text)
            throw new BadRequestException("Only text messages can be edited.");

        if (message.DeletedForEveryoneAt is not null)
            throw new BadRequestException("This message was deleted for everyone.");

        var content = request.Content?.Trim();
        if (string.IsNullOrWhiteSpace(content))
            throw new BadRequestException("Message content is required.");

        if (content.Length > 5000)
            throw new BadRequestException("Message content cannot exceed 5000 characters.");

        message.Content = content;
        message.IsEdited = true;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await chatQueryService.GetMessageAsync(message.Id, userId, cancellationToken)
            ?? throw new NotFoundException(nameof(ChatMessage), message.Id);

        return ApiResponse<MessageDto>.Success(dto, "Message updated.");
    }
}
