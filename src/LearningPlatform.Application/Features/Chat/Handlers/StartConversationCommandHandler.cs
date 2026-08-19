using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Application.Features.Chat.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class StartConversationCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    UserManager<ApplicationUser> userManager,
    IChatQueryService chatQueryService)
    : IRequestHandler<StartConversationCommand, ApiResponse<ConversationDto>>
{
    public async Task<ApiResponse<ConversationDto>> Handle(StartConversationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        if (request.OtherUserId == userId)
            throw new BadRequestException("You cannot start a conversation with yourself.");

        var otherUser = await userManager.FindByIdAsync(request.OtherUserId.ToString())
            ?? throw new NotFoundException(nameof(ApplicationUser), request.OtherUserId);

        var isBlockedByOther = await unitOfWork.Repository<BlockedUser>()
            .ExistsAsync(b => b.UserId == request.OtherUserId && b.BlockedUserId == userId, cancellationToken);

        if (isBlockedByOther)
            throw new ForbiddenException("You cannot start a conversation with this user.");

        var conversationId = await unitOfWork.Repository<Conversation>().AsQueryable()
            .Where(c => c.Participants.Any(p => p.UserId == userId)
                     && c.Participants.Any(p => p.UserId == request.OtherUserId))
            .Select(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (conversationId == Guid.Empty)
        {
            var conversation = new Conversation();
            await unitOfWork.Repository<Conversation>().AddAsync(conversation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await unitOfWork.Repository<ConversationParticipant>().AddAsync(
                new ConversationParticipant { ConversationId = conversation.Id, UserId = userId }, cancellationToken);
            await unitOfWork.Repository<ConversationParticipant>().AddAsync(
                new ConversationParticipant { ConversationId = conversation.Id, UserId = request.OtherUserId }, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            conversationId = conversation.Id;
        }
        else
        {
            var myParticipant = await unitOfWork.Repository<ConversationParticipant>()
                .GetTrackedAsync(p => p.ConversationId == conversationId && p.UserId == userId, cancellationToken);

            if (myParticipant is { IsHidden: true })
            {
                myParticipant.IsHidden = false;
                myParticipant.HiddenAt = null;
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        var dto = await chatQueryService.GetConversationAsync(conversationId, userId, cancellationToken);
        return ApiResponse<ConversationDto>.Success(dto, "Conversation started.");
    }
}
