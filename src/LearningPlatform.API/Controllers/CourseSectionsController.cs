using Asp.Versioning;
using LearningPlatform.Application.Features.CourseSections.Commands;
using LearningPlatform.Application.Features.CourseSections.DTOs;
using LearningPlatform.Application.Features.CourseSections.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/courses/{courseId:guid}/sections")]
public class CourseSectionsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<SectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSection(Guid courseId, [FromBody] CreateSectionCommand command)
    {
        var result = await mediator.Send(command with { CourseId = courseId });
        return Ok(result);
    }

    [HttpPut("{sectionId:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<SectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSection(Guid courseId, Guid sectionId, [FromBody] UpdateSectionCommand command)
    {
        var result = await mediator.Send(command with { Id = sectionId });
        return Ok(result);
    }

    [HttpDelete("{sectionId:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSection(Guid courseId, Guid sectionId)
    {
        var result = await mediator.Send(new DeleteSectionCommand(sectionId));
        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<SectionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSections(Guid courseId)
    {
        var result = await mediator.Send(new GetSectionsQuery(courseId));
        return Ok(result);
    }
}
