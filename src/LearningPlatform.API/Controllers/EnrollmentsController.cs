using Asp.Versioning;
using LearningPlatform.Application.Features.Enrollments.Commands;
using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Application.Features.Enrollments.Queries;
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
public class EnrollmentsController(IMediator mediator) : ControllerBase
{
    [HttpPost("CreateEnrollment")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("MyEnrollments")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(PaginatedResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyEnrollments([FromQuery] GetMyEnrollmentsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetEnrollment/{courseId:guid}")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollment(Guid courseId)
    {
        var result = await mediator.Send(new GetEnrollmentQuery(courseId));
        return Ok(result);
    }

    [HttpDelete("CancelEnrollment/{courseId:guid}")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelEnrollment(Guid courseId)
    {
        var result = await mediator.Send(new CancelEnrollmentCommand(courseId));
        return Ok(result);
    }

    [HttpGet("CourseStudents/{courseId:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(PaginatedResponse<CourseStudentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CourseStudents(Guid courseId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetCourseStudentsQuery
        {
            CourseId = courseId,
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return Ok(result);
    }

    [HttpGet("GetAllEnrollments")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(PaginatedResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllEnrollments([FromQuery] GetAllEnrollmentsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("ByStudent/{studentId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<EnrollmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentEnrollments(Guid studentId)
    {
        var result = await mediator.Send(new GetStudentEnrollmentsQuery(studentId));
        return Ok(result);
    }
}
