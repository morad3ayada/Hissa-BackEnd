namespace LearningPlatform.Application.Features.Reports.DTOs;

public class QuizzesReportDto
{
    public List<DifficultQuizDto> MostDifficultQuizzes { get; set; } = [];
    public int TotalAttempts { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public decimal PassRate { get; set; }
    public decimal FailRate { get; set; }
}

public class DifficultQuizDto
{
    public Guid QuizId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TotalAttempts { get; set; }
    public int PassedCount { get; set; }
    public decimal PassRate { get; set; }
}
