using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Commands;

public record AddQuestionsCommand : IRequest<ApiResponse<List<QuestionDto>>>
{
    public Guid QuizId { get; init; }
    public List<QuestionInput> Questions { get; init; } = [];
}

public record QuestionInput
{
    public string Text { get; init; } = string.Empty;
    public string? Explanation { get; init; }
    public int Points { get; init; } = 1;

    /// <summary>
    /// SingleChoice | MultipleChoice | TrueFalse | ShortAnswer | Essay
    /// </summary>
    public string Type { get; init; } = "SingleChoice";

    public List<AnswerOptionInput> Answers { get; init; } = [];
}
