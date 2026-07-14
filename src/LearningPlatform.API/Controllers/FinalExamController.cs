using Asp.Versioning;
using LearningPlatform.Application.Features.FinalExam.Queries;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FinalExamController(IMediator mediator) : ControllerBase
{
    [HttpGet("{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<QuizDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinalExam(Guid courseId)
    {
        var result = await mediator.Send(new GetFinalExamQuery(courseId));
        return Ok(result);
    }
}
