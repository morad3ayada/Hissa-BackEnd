using Asp.Versioning;
using LearningPlatform.Application.Features.Subscriptions.Commands;
using LearningPlatform.Application.Features.Subscriptions.DTOs;
using LearningPlatform.Application.Features.Subscriptions.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SubscriptionPlansController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<SubscriptionPlanDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans()
    {
        var result = await mediator.Send(new GetSubscriptionPlansQuery());
        return Ok(result);
    }
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = nameof(UserRole.Instructor))]
public class InstructorSubscriptionsController(IMediator mediator) : ControllerBase
{
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<InstructorSubscriptionDto?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMySubscription()
    {
        var result = await mediator.Send(new GetMySubscriptionQuery());
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InstructorSubscriptionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("renew")]
    [ProducesResponseType(typeof(ApiResponse<InstructorSubscriptionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Renew([FromBody] RenewSubscriptionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("cancel")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel()
    {
        var result = await mediator.Send(new CancelSubscriptionCommand());
        return Ok(result);
    }
}
