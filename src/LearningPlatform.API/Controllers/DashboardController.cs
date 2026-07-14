using Asp.Versioning;
using LearningPlatform.Application.Features.Dashboard.DTOs;
using LearningPlatform.Application.Features.Dashboard.Queries;
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
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("Admin")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse<AdminDashboardDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Admin()
    {
        var result = await mediator.Send(new GetAdminDashboardQuery());
        return Ok(result);
    }

    [HttpGet("Instructor")]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(typeof(ApiResponse<InstructorDashboardDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Instructor()
    {
        var result = await mediator.Send(new GetInstructorDashboardQuery());
        return Ok(result);
    }

    [HttpGet("Student")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<StudentDashboardDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Student()
    {
        var result = await mediator.Send(new GetStudentDashboardQuery());
        return Ok(result);
    }

    [HttpGet("Parent")]
    [Authorize(Roles = nameof(UserRole.Parent))]
    [ProducesResponseType(typeof(ApiResponse<ParentDashboardDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Parent()
    {
        var result = await mediator.Send(new GetParentDashboardQuery());
        return Ok(result);
    }
}
