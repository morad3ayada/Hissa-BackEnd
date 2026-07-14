using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Commands;

public record SubmitAnswerInput(Guid QuestionId, Guid? SelectedAnswerId);

/// <summary>
/// StartedAt is client-reported (captured when the student opened the quiz via GetQuiz) since
/// there is no separate "start attempt" endpoint in the API surface; used only for time-limit
/// enforcement, with a small grace window for clock skew and network latency.
/// </summary>
public record SubmitQuizCommand(Guid QuizId, DateTime StartedAt, List<SubmitAnswerInput> Answers)
    : IRequest<ApiResponse<QuizResultDto>>;
