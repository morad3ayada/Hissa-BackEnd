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
public class TeacherProfileController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<TeacherProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyProfile()
    {
        var result = await mediator.Send(new GetTeacherProfileQuery());
        return Ok(result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<TeacherProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateTeacherProfileCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
