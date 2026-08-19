using Asp.Versioning;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Application.Features.Chat.Queries;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ChatController(IMediator mediator) : ControllerBase
{
    [HttpPost("conversations/start")]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        var result = await mediator.Send(new StartConversationCommand(request.OtherUserId));
        return Ok(result);
    }

    [HttpGet("conversations")]
    [ProducesResponseType(typeof(PaginatedResponse<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyConversations([FromQuery] GetMyConversationsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("conversations/{conversationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversation(Guid conversationId)
    {
        var result = await mediator.Send(new GetConversationQuery(conversationId));
        return Ok(result);
    }

    [HttpGet("conversations/{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(PaginatedResponse<MessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessages(
        Guid conversationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 30,
        [FromQuery] Guid? beforeMessageId = null)
    {
        var result = await mediator.Send(new GetConversationMessagesQuery
        {
            ConversationId = conversationId,
            PageNumber = pageNumber,
            PageSize = pageSize,
            BeforeMessageId = beforeMessageId
        });
        return Ok(result);
    }

    [HttpPost("conversations/{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendMessageRequest request)
    {
        var result = await mediator.Send(new SendMessageCommand
        {
            ConversationId = conversationId,
            Content = request.Content,
            ReplyToMessageId = request.ReplyToMessageId,
            Attachments = request.Attachments ?? []
        });
        return Ok(result);
    }

    [HttpPut("conversations/{conversationId:guid}/read")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkConversationAsRead(Guid conversationId, [FromBody] MarkAsReadRequest request)
    {
        var result = await mediator.Send(new MarkConversationAsReadCommand(conversationId, request.LastMessageId));
        return Ok(result);
    }

    [HttpDelete("conversations/{conversationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteConversation(Guid conversationId)
    {
        var result = await mediator.Send(new DeleteConversationCommand(conversationId));
        return Ok(result);
    }

    [HttpPut("conversations/{conversationId:guid}/mute")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetConversationMuted(Guid conversationId, [FromBody] SetMutedRequest request)
    {
        var result = await mediator.Send(new SetConversationMutedCommand(conversationId, request.IsMuted));
        return Ok(result);
    }

    [HttpPut("messages/{messageId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> EditMessage(Guid messageId, [FromBody] EditMessageRequest request)
    {
        var result = await mediator.Send(new EditMessageCommand(messageId, request.Content));
        return Ok(result);
    }

    [HttpDelete("messages/{messageId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMessage(Guid messageId, [FromQuery] bool forEveryone = false)
    {
        var result = await mediator.Send(new DeleteMessageCommand(messageId, forEveryone));
        return Ok(result);
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<UnreadCountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var result = await mediator.Send(new GetUnreadCountQuery());
        return Ok(result);
    }

    [HttpPost("attachments")]
    [ProducesResponseType(typeof(ApiResponse<UploadedAttachmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadAttachment(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var result = await mediator.Send(new UploadChatAttachmentCommand
        {
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length
        });
        return Ok(result);
    }

    [HttpPost("block")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> BlockUser([FromBody] BlockUserRequest request)
    {
        var result = await mediator.Send(new BlockUserCommand(request.UserId));
        return Ok(result);
    }

    [HttpDelete("block/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UnblockUser(Guid userId)
    {
        var result = await mediator.Send(new UnblockUserCommand(userId));
        return Ok(result);
    }

    [HttpGet("blocked")]
    [ProducesResponseType(typeof(PaginatedResponse<BlockedUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBlockedUsers([FromQuery] GetBlockedUsersQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    public record StartConversationRequest(Guid OtherUserId);

    public record SendMessageRequest(
        string? Content,
        Guid? ReplyToMessageId,
        List<MessageAttachmentInput>? Attachments);

    public record MarkAsReadRequest(Guid? LastMessageId);

    public record SetMutedRequest(bool IsMuted);

    public record EditMessageRequest(string Content);

    public record BlockUserRequest(Guid UserId);
}
