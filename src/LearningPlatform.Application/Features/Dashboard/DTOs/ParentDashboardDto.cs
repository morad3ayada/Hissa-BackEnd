namespace LearningPlatform.Application.Features.Dashboard.DTOs;

public class ParentDashboardDto
{
    public List<ChildDashboardDto> Children { get; set; } = [];
}

public class ChildDashboardDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal OverallProgressPercentage { get; set; }
    public List<QuizScoreDto> QuizScores { get; set; } = [];
    public int ErrorsCount { get; set; }
    public double StudyHours { get; set; }
    public DateTime? LastActivityAt { get; set; }
}

public class QuizScoreDto
{
    public string QuizTitle { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public bool IsPassed { get; set; }
    public DateTime CompletedAt { get; set; }
}
