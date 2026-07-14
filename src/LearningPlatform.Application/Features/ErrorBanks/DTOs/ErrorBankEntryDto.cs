using LearningPlatform.Application.Features.Quizzes.DTOs;

namespace LearningPlatform.Application.Features.ErrorBanks.DTOs;

/// <summary>Practice-queue entry: exposes the question and answer options WITHOUT the answer key,
/// so retrying it is a genuine re-attempt rather than a spoiled lookup.</summary>
public class ErrorBankEntryDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<AnswerDto> Answers { get; set; } = [];
    public int MistakeCount { get; set; }
    public DateTime LastMistakeAt { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Guid? LessonId { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseTitle { get; set; }
}
