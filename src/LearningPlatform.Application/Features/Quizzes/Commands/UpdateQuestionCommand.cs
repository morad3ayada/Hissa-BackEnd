using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Commands;

public record UpdateQuestionCommand : IRequest<ApiResponse<QuestionDto>>
{
    public Guid QuestionId { get; init; }
    public string Text { get; init; } = string.Empty;
    public string? Explanation { get; init; }
    public int Points { get; init; } = 1;
    public List<UpdateAnswerInput> Answers { get; init; } = [];
}

public record UpdateAnswerInput
{
    public Guid Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
}
