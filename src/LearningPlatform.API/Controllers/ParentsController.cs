using Asp.Versioning;
using LearningPlatform.Application.Features.Parents.Commands;
using LearningPlatform.Application.Features.Parents.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ParentsController(IMediator mediator) : ControllerBase
{
    [HttpPost("LinkStudent")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LinkStudent([FromBody] LinkParentToStudentCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("LinkMyStudent")]
    [Authorize(Roles = nameof(UserRole.Parent))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LinkMyStudent([FromBody] LinkMyStudentCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("MyChildren")]
    [Authorize(Roles = nameof(UserRole.Parent))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyChildren()
    {
        var result = await mediator.Send(new GetMyChildrenQuery());
        return Ok(result);
    }

    [HttpGet("Children/{studentId:guid}/Enrollments")]
    [Authorize(Roles = nameof(UserRole.Parent))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChildEnrollments(Guid studentId)
    {
        var result = await mediator.Send(new GetChildEnrollmentsQuery(studentId));
        return Ok(result);
    }
}
