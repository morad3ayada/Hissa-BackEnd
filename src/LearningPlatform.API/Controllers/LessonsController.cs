using Asp.Versioning;
using LearningPlatform.Application.Features.Lessons.Commands;
using LearningPlatform.Application.Features.Lessons.DTOs;
using LearningPlatform.Application.Features.Lessons.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

// Video size limits below must stay in sync with FileStorageSettings:MaxVideoSizeInBytes (appsettings.json).
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sections/{sectionId:guid}/lessons")]
public class LessonsController(IMediator mediator) : ControllerBase
{
    private const long MaxVideoSizeInBytes = 2L * 1024 * 1024 * 1024;

    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<LessonDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateLesson(Guid sectionId, [FromBody] CreateLessonCommand command)
    {
        var result = await mediator.Send(command with { CourseSectionId = sectionId });
        return Ok(result);
    }

    [HttpPut("{lessonId:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<LessonDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateLesson(Guid sectionId, Guid lessonId, [FromBody] UpdateLessonCommand command)
    {
        var result = await mediator.Send(command with { Id = lessonId });
        return Ok(result);
    }

    [HttpDelete("{lessonId:guid}")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteLesson(Guid sectionId, Guid lessonId)
    {
        var result = await mediator.Send(new DeleteLessonCommand(lessonId));
        return Ok(result);
    }

    [HttpGet("{lessonId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LessonDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLesson(Guid sectionId, Guid lessonId)
    {
        var result = await mediator.Send(new GetLessonQuery(lessonId));
        return Ok(result);
    }

    [HttpPost("{lessonId:guid}/video")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxVideoSizeInBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxVideoSizeInBytes)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadVideo(Guid sectionId, Guid lessonId, IFormFile file, [FromForm] int? durationInSeconds)
    {
        await using var stream = file.OpenReadStream();

        var result = await mediator.Send(new UploadVideoCommand
        {
            LessonId = lessonId,
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            DurationInSeconds = durationInSeconds
        });

        return Ok(result);
    }

    [HttpPut("{lessonId:guid}/video")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxVideoSizeInBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxVideoSizeInBytes)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReplaceVideo(Guid sectionId, Guid lessonId, IFormFile file, [FromForm] int? durationInSeconds)
    {
        await using var stream = file.OpenReadStream();

        var result = await mediator.Send(new ReplaceVideoCommand
        {
            LessonId = lessonId,
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            DurationInSeconds = durationInSeconds
        });

        return Ok(result);
    }

    [HttpDelete("{lessonId:guid}/video")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteVideo(Guid sectionId, Guid lessonId)
    {
        var result = await mediator.Send(new DeleteVideoCommand(lessonId));
        return Ok(result);
    }

    [HttpGet("{lessonId:guid}/video")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    public async Task<IActionResult> StreamVideo(Guid sectionId, Guid lessonId)
    {
        var result = await mediator.Send(new GetVideoStreamQuery(lessonId));
        return File(result.Stream, result.ContentType, result.FileName, enableRangeProcessing: true);
    }
}
