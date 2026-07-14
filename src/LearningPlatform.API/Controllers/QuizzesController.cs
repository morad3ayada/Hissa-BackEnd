using Asp.Versioning;
using LearningPlatform.Application.Features.Quizzes.Commands;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class QuizzesController(IMediator mediator) : ControllerBase
{
    [HttpPost("CreateQuiz")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<QuizDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddQuestion")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<QuestionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddQuestion([FromBody] AddQuestionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("UpdateQuestion")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<QuestionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateQuestion([FromBody] UpdateQuestionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("DeleteQuestion")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteQuestion([FromQuery] Guid questionId)
    {
        var result = await mediator.Send(new DeleteQuestionCommand(questionId));
        return Ok(result);
    }

    [HttpPost("PublishQuiz")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<QuizDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PublishQuiz([FromBody] PublishQuizCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetQuiz/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<QuizDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuiz(Guid id)
    {
        var result = await mediator.Send(new GetQuizQuery(id));
        return Ok(result);
    }

    [HttpGet("GetLessonQuizzes/{lessonId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<QuizSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLessonQuizzes(Guid lessonId)
    {
        var result = await mediator.Send(new GetLessonQuizzesQuery(lessonId));
        return Ok(result);
    }

    [HttpPost("{quizId:guid}/Submit")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<QuizResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Submit(Guid quizId, [FromBody] SubmitQuizCommand command)
    {
        var result = await mediator.Send(command with { QuizId = quizId });
        return Ok(result);
    }

    [HttpGet("GetResult/{attemptId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<QuizResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResult(Guid attemptId)
    {
        var result = await mediator.Send(new GetResultQuery(attemptId));
        return Ok(result);
    }
}
