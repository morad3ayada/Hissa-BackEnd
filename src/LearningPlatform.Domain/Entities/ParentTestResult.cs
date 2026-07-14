using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class ParentTestResult : BaseEntity
{
    public decimal? Score { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string? Feedback { get; set; }

    public Guid ParentTestId { get; set; }
    public ParentTest ParentTest { get; set; } = null!;

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;
}
