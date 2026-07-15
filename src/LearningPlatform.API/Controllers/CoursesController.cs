using Asp.Versioning;
using LearningPlatform.Application.Features.Courses.Commands;
using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Application.Features.Courses.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CoursesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var result = await mediator.Send(new DeleteCourseCommand(id));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseById(Guid id)
    {
        var result = await mediator.Send(new GetCourseByIdQuery(id));
        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<CourseSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCourses()
    {
        var result = await mediator.Send(new GetAllCoursesQuery());
        return Ok(result);
    }

    [HttpGet("MyChildren")]
    [Authorize(Roles = nameof(UserRole.Parent))]
    [ProducesResponseType(typeof(ApiResponse<List<CourseSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyChildrenCourses()
    {
        var result = await mediator.Send(new GetMyChildrenCoursesQuery());
        return Ok(result);
    }

    [HttpGet("MyEnrolled")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<List<CourseSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyEnrolledCourses()
    {
        var result = await mediator.Send(new GetMyEnrolledCoursesQuery());
        return Ok(result);
    }

    [HttpGet("admin")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse<List<CourseSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCoursesForAdmin()
    {
        var result = await mediator.Send(new GetCoursesForAdminQuery());
        return Ok(result);
    }

    [HttpPatch("{id:guid}/state")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetCourseState(Guid id, [FromBody] SetCourseStateCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return Ok(result);
    }

    [HttpPost("{id:guid}/thumbnail")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadThumbnail(Guid id, IFormFile file)
    {
        await using var stream = file.OpenReadStream();

        var result = await mediator.Send(new UploadCourseThumbnailCommand
        {
            CourseId = id,
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length
        });

        return Ok(result);
    }
}
