using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Commands;

public record CreateQuizCommand : IRequest<ApiResponse<QuizDto>>
{
    public string Title { get; init; } = string.Empty;
    public QuizScope Scope { get; init; }
    public Guid? CourseId { get; init; }
    public Guid? LessonId { get; init; }
    public bool IsFinalExam { get; init; }
    public int? TimeLimitInMinutes { get; init; }
    public int PassingScore { get; init; } = 60;
    public int? MaxAttempts { get; init; }
}
