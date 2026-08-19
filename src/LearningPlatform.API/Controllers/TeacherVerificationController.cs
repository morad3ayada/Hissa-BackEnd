using Asp.Versioning;
using LearningPlatform.Application.Features.TeacherProfiles.Commands;
using LearningPlatform.Application.Features.TeacherProfiles.DTOs;
using LearningPlatform.Application.Features.TeacherProfiles.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/TeacherProfile")]
[Authorize(Roles = nameof(UserRole.Instructor))]
public class TeacherVerificationController(IMediator mediator) : ControllerBase
{
    [HttpGet("verification-status")]
    [ProducesResponseType(typeof(ApiResponse<VerificationStatusDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVerificationStatus()
    {
        var result = await mediator.Send(new GetVerificationStatusQuery());
        return Ok(result);
    }

    [HttpPost("verification/resubmit")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResubmitVerification()
    {
        var result = await mediator.Send(new ResubmitVerificationCommand());
        return Ok(result);
    }

    [HttpPut("booking-status")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBookingStatus([FromBody] UpdateBookingStatusCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/Admin/Teachers")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminTeacherVerificationController(IMediator mediator) : ControllerBase
{
    [HttpGet("pending-verification")]
    [ProducesResponseType(typeof(ApiResponse<List<PendingTeacherDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingTeachers()
    {
        var result = await mediator.Send(new GetPendingTeachersQuery());
        return Ok(result);
    }

    [HttpPut("{teacherProfileId:guid}/approve")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveTeacher(Guid teacherProfileId)
    {
        var result = await mediator.Send(new ApproveTeacherCommand(teacherProfileId));
        return Ok(result);
    }

    [HttpPut("{teacherProfileId:guid}/reject")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectTeacher(Guid teacherProfileId, [FromBody] RejectTeacherCommand command)
    {
        var result = await mediator.Send(command with { TeacherProfileId = teacherProfileId });
        return Ok(result);
    }
}
