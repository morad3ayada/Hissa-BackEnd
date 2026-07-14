using Asp.Versioning;
using LearningPlatform.Application.Features.Notifications.Commands;
using LearningPlatform.Application.Features.Notifications.DTOs;
using LearningPlatform.Application.Features.Notifications.Queries;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    [HttpGet("MyNotifications")]
    [ProducesResponseType(typeof(PaginatedResponse<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyNotifications([FromQuery] GetMyNotificationsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("UnreadCount")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UnreadCount()
    {
        var result = await mediator.Send(new GetUnreadCountQuery());
        return Ok(result);
    }

    [HttpPut("MarkAsRead/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await mediator.Send(new MarkAsReadCommand(id));
        return Ok(result);
    }

    [HttpPut("MarkAllAsRead")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var result = await mediator.Send(new MarkAllAsReadCommand());
        return Ok(result);
    }

    [HttpDelete("Delete/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteNotificationCommand(id));
        return Ok(result);
    }
}
