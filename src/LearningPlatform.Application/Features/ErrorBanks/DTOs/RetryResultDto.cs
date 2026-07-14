namespace LearningPlatform.Application.Features.ErrorBanks.DTOs;

/// <summary>Post-answer feedback for a single retried question — the answer key is only
/// revealed here, after the student has already committed a choice.</summary>
public class RetryResultDto
{
    public Guid QuestionId { get; set; }
    public bool IsCorrect { get; set; }
    public bool IsResolved { get; set; }
    public Guid? CorrectAnswerId { get; set; }
    public string? CorrectAnswerText { get; set; }
    public string? Explanation { get; set; }
}
