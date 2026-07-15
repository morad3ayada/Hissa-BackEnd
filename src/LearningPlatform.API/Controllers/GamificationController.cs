using Asp.Versioning;
using LearningPlatform.Application.Features.Gamification.Commands;
using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Application.Features.Gamification.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Instructor)}")]
public class GamificationController(IMediator mediator) : ControllerBase
{
    [HttpGet("Profile")]
    [ProducesResponseType(typeof(ApiResponse<GamificationProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Profile()
    {
        var result = await mediator.Send(new GetProfileQuery());
        return Ok(result);
    }

    [HttpGet("Leaderboard")]
    [ProducesResponseType(typeof(PaginatedResponse<LeaderboardEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Leaderboard([FromQuery] GetLeaderboardQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("AvatarStore")]
    [ProducesResponseType(typeof(ApiResponse<List<AvatarItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AvatarStore()
    {
        var result = await mediator.Send(new GetAvatarStoreQuery());
        return Ok(result);
    }

    [HttpPost("BuyAvatarItem")]
    [ProducesResponseType(typeof(ApiResponse<AvatarItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BuyAvatarItem([FromBody] BuyAvatarItemCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("CreateChallenge")]
    [ProducesResponseType(typeof(ApiResponse<ChallengeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateChallenge([FromBody] CreateChallengeCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AcceptChallenge")]
    [ProducesResponseType(typeof(ApiResponse<ChallengeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AcceptChallenge([FromBody] AcceptChallengeCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("MigrateAvatarImages")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MigrateAvatarImages()
    {
        var result = await mediator.Send(new MigrateAvatarImagesCommand());
        return Ok(result);
    }

    [HttpPost("SubmitChallenge")]
    [ProducesResponseType(typeof(ApiResponse<ChallengeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitChallenge([FromBody] SubmitChallengeCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("MyRewards")]
    [ProducesResponseType(typeof(ApiResponse<List<StudentRewardDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyRewards()
    {
        var result = await mediator.Send(new GetMyRewardsQuery());
        return Ok(result);
    }

    [HttpGet("MyAchievements")]
    [ProducesResponseType(typeof(ApiResponse<List<StudentRewardDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyAchievements()
    {
        var result = await mediator.Send(new GetMyAchievementsQuery());
        return Ok(result);
    }
}
