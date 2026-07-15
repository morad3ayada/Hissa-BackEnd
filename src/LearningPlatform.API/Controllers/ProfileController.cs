using Asp.Versioning;
using LearningPlatform.Application.Features.Profiles.Commands;
using LearningPlatform.Application.Features.Profiles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ProfileController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyProfile()
    {
        var result = await mediator.Send(new GetMyProfileQuery());
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("picture")]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var result = await mediator.Send(new UploadProfilePictureCommand
        {
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length
        });
        return Ok(result);
    }
}
