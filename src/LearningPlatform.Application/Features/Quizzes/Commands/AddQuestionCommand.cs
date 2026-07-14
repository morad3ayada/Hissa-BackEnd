using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Commands;

public record AddQuestionCommand : IRequest<ApiResponse<QuestionDto>>
{
    public Guid QuizId { get; init; }
    public string Text { get; init; } = string.Empty;
    public string? Explanation { get; init; }
    public int Points { get; init; } = 1;
    public List<AnswerOptionInput> Answers { get; init; } = [];
}

public record AnswerOptionInput
{
    public string Text { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
}
