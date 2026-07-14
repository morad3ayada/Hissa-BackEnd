using Asp.Versioning;
using LearningPlatform.Application.Features.Reports.DTOs;
using LearningPlatform.Application.Features.Reports.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class ReportsController(IMediator mediator) : ControllerBase
{
    [HttpGet("Courses")]
    [ProducesResponseType(typeof(ApiResponse<CoursesReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Courses()
    {
        var result = await mediator.Send(new GetCoursesReportQuery());
        return Ok(result);
    }

    [HttpGet("Students")]
    [ProducesResponseType(typeof(ApiResponse<StudentsReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Students()
    {
        var result = await mediator.Send(new GetStudentsReportQuery());
        return Ok(result);
    }

    [HttpGet("Quizzes")]
    [ProducesResponseType(typeof(ApiResponse<QuizzesReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Quizzes()
    {
        var result = await mediator.Send(new GetQuizzesReportQuery());
        return Ok(result);
    }

    [HttpGet("Payments")]
    [ProducesResponseType(typeof(ApiResponse<PaymentsReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Payments()
    {
        var result = await mediator.Send(new GetPaymentsReportQuery());
        return Ok(result);
    }
}
