using Asp.Versioning;
using LearningPlatform.Application.Features.LiveSessions.Commands;
using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Application.Features.LiveSessions.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class LiveSessionsController(IMediator mediator) : ControllerBase
{
    [HttpPost("Create")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<LiveSessionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateLiveSessionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("Update/{id:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<LiveSessionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLiveSessionCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return Ok(result);
    }

    [HttpDelete("Delete/{id:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteLiveSessionCommand(id));
        return Ok(result);
    }

    [HttpGet("GetById/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LiveSessionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetLiveSessionByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("GetUpcoming")]
    [ProducesResponseType(typeof(PaginatedResponse<LiveSessionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcoming([FromQuery] GetUpcomingLiveSessionsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetPrevious")]
    [ProducesResponseType(typeof(PaginatedResponse<LiveSessionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPrevious([FromQuery] GetPreviousLiveSessionsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetCourseSessions/{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<LiveSessionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseSessions(Guid courseId)
    {
        var result = await mediator.Send(new GetCourseSessionsQuery(courseId));
        return Ok(result);
    }

    [HttpGet("GetInstructorSessions")]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(typeof(PaginatedResponse<LiveSessionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstructorSessions([FromQuery] GetInstructorSessionsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    // POST: Record student join
    [HttpPost("Join/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LiveSessionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Join(Guid id)
    {
        var result = await mediator.Send(new JoinLiveSessionCommand(id));
        return Ok(result);
    }

    // GET: Attendance list (Instructor/Admin only)
    [HttpGet("{id:guid}/Attendance")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<List<LiveSessionAttendanceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttendance(Guid id)
    {
        var result = await mediator.Send(new GetLiveSessionAttendanceQuery(id));
        return Ok(result);
    }
}
