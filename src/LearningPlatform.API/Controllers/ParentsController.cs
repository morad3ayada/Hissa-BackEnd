using Asp.Versioning;
using LearningPlatform.Application.Features.Parents.Commands;
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
public class ParentsController(IMediator mediator) : ControllerBase
{
    [HttpPost("LinkStudent")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LinkStudent([FromBody] LinkParentToStudentCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
