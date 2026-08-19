using Asp.Versioning;
using LearningPlatform.Application.Features.Instructors.DTOs;
using LearningPlatform.Application.Features.Instructors.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = nameof(UserRole.Student))]
public class InstructorsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<InstructorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstructors([FromQuery] GetInstructorsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
