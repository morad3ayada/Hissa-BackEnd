using System.Security.Claims;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Application.Features.Chat.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.API.Hubs;

[Authorize]
public class ChatHub(IServiceScopeFactory scopeFactory, IPresenceTracker presenceTracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId is not null)
        {
            await presenceTracker.AddConnectionAsync(userId.Value, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId.Value));
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId is not null)
        {
            await presenceTracker.RemoveConnectionAsync(userId.Value, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserGroup(userId.Value));
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(SendMessageCommand command)
    {
        var result = await ExecuteAsync(sender => sender.Send(command, Context.ConnectionAborted));

        if (result?.Succeeded == true && result.Data is not null)
        {
            await Clients.Group(ConversationGroup(result.Data.ConversationId))
                .SendAsync("ReceiveMessage", result.Data, Context.ConnectionAborted);
        }
    }

    public async Task EditMessage(EditMessageCommand command)
    {
        var result = await ExecuteAsync(sender => sender.Send(command, Context.ConnectionAborted));

        if (result?.Succeeded == true && result.Data is not null)
        {
            await Clients.Group(ConversationGroup(result.Data.ConversationId))
                .SendAsync("MessageEdited", result.Data, Context.ConnectionAborted);
        }
    }

    public async Task DeleteMessage(Guid messageId, bool forEveryone)
    {
        var result = await ExecuteAsync(sender => sender.Send(new DeleteMessageCommand(messageId, forEveryone), Context.ConnectionAborted));

        if (result?.Succeeded == true)
        {
            // The message is already persisted; fetch the conversation id so other devices
            // can drop it from their local list.
            var conversationId = await GetConversationIdForMessageAsync(messageId);
            if (conversationId is not null)
            {
                await Clients.Group(ConversationGroup(conversationId.Value))
                    .SendAsync("MessageDeleted", new MessageDeletedEvent(conversationId.Value, messageId, forEveryone), Context.ConnectionAborted);
            }
        }
    }

    public async Task MarkConversationAsRead(Guid conversationId, Guid? lastMessageId)
    {
        var result = await ExecuteAsync(sender =>
            sender.Send(new MarkConversationAsReadCommand(conversationId, lastMessageId), Context.ConnectionAborted));

        if (result?.Succeeded == true)
        {
            var userId = GetUserId();
            await Clients.Group(ConversationGroup(conversationId))
                .SendAsync("MessagesRead", new ReadReceiptEvent(conversationId, userId, result.Data, lastMessageId), Context.ConnectionAborted);
        }
    }

    public async Task ConfirmDelivered(Guid conversationId, List<Guid> messageIds)
    {
        var result = await ExecuteAsync(sender =>
            sender.Send(new MarkMessagesDeliveredCommand(conversationId, messageIds), Context.ConnectionAborted));

        if (result?.Succeeded == true)
        {
            await Clients.Group(ConversationGroup(conversationId))
                .SendAsync("MessageDelivered", new DeliveredReceiptEvent(conversationId, messageIds), Context.ConnectionAborted);
        }
    }

    public async Task Typing(Guid conversationId, bool isTyping)
    {
        var userId = GetUserId();
        if (userId is null)
            return;

        var result = await ExecuteAsync(sender =>
            sender.Send(new IsConversationParticipantQuery(conversationId), Context.ConnectionAborted));

        if (result?.Succeeded == true && result.Data)
        {
            await Clients.GroupExcept(ConversationGroup(conversationId), Context.ConnectionId)
                .SendAsync("UserTyping", new TypingEvent(conversationId, userId.Value, isTyping), Context.ConnectionAborted);
        }
    }

    public async Task JoinConversation(Guid conversationId)
    {
        var result = await ExecuteAsync(sender =>
            sender.Send(new IsConversationParticipantQuery(conversationId), Context.ConnectionAborted));

        if (result?.Succeeded == true && result.Data)
            await Groups.AddToGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
    }

    public Task LeaveConversation(Guid conversationId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));

    private async Task<Guid?> GetConversationIdForMessageAsync(Guid messageId)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var message = await unitOfWork.Repository<LearningPlatform.Domain.Entities.ChatMessage>()
            .AsQueryable()
            .Where(m => m.Id == messageId)
            .Select(m => new { m.ConversationId })
            .FirstOrDefaultAsync();

        return message?.ConversationId;
    }

    private Guid? GetUserId()
    {
        var id = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var userId) ? userId : null;
    }

    private async Task<T?> ExecuteAsync<T>(Func<ISender, Task<T>> action)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return await action(sender);
    }

    private static string UserGroup(Guid userId) => $"user_{userId}";
    private static string ConversationGroup(Guid conversationId) => $"conversation_{conversationId}";

    public record TypingEvent(Guid ConversationId, Guid UserId, bool IsTyping);

    public record ReadReceiptEvent(Guid ConversationId, Guid? UserId, int Count, Guid? LastMessageId);

    public record DeliveredReceiptEvent(Guid ConversationId, List<Guid> MessageIds);

    public record MessageDeletedEvent(Guid ConversationId, Guid MessageId, bool ForEveryone);
}
