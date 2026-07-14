using Asp.Versioning;
using LearningPlatform.Application.Features.ErrorBanks.Commands;
using LearningPlatform.Application.Features.ErrorBanks.DTOs;
using LearningPlatform.Application.Features.ErrorBanks.Queries;
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
public class ErrorBankController(IMediator mediator) : ControllerBase
{
    [HttpGet("MyErrors")]
    [ProducesResponseType(typeof(ApiResponse<List<ErrorBankEntryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyErrors([FromQuery] bool includeResolved = false)
    {
        var result = await mediator.Send(new GetMyErrorsQuery(includeResolved));
        return Ok(result);
    }

    [HttpPost("Retry")]
    [ProducesResponseType(typeof(ApiResponse<List<RetryResultDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Retry([FromBody] RetryErrorBankCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
