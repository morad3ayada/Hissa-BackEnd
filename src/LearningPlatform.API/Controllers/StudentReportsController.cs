using Asp.Versioning;
using LearningPlatform.Application.Features.StudentReports.Commands;
using LearningPlatform.Application.Features.StudentReports.DTOs;
using LearningPlatform.Application.Features.StudentReports.Queries;
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
public class StudentReportsController(IMediator mediator) : ControllerBase
{
    [HttpPost("Create")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<StudentReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateStudentReportCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("ByStudent/{studentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<StudentReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ByStudent(Guid studentId)
    {
        var result = await mediator.Send(new GetChildStudentReportsQuery(studentId));
        return Ok(result);
    }
}
