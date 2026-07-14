using Asp.Versioning;
using LearningPlatform.Application.Features.Progress.Commands;
using LearningPlatform.Application.Features.Progress.DTOs;
using LearningPlatform.Application.Features.Progress.Queries;
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
public class ProgressController(IMediator mediator) : ControllerBase
{
    [HttpPost("UpdateLessonProgress")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<LessonProgressDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateLessonProgress([FromBody] UpdateLessonProgressCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetMyCourseProgress/{courseId:guid}")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<CourseProgressDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyCourseProgress(Guid courseId)
    {
        var result = await mediator.Send(new GetMyCourseProgressQuery(courseId));
        return Ok(result);
    }

    [HttpGet("GetMyCoursesProgress")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(PaginatedResponse<CourseProgressSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyCoursesProgress([FromQuery] GetMyCoursesProgressQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("ContinueLearning")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<List<ContinueLearningDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ContinueLearning()
    {
        var result = await mediator.Send(new GetContinueLearningQuery());
        return Ok(result);
    }

    [HttpGet("GetStudentProgress/{studentId:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<List<CourseProgressSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentProgress(Guid studentId)
    {
        var result = await mediator.Send(new GetStudentProgressQuery(studentId));
        return Ok(result);
    }
}
