using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class ParentTest : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? DueDate { get; set; }

    public Guid ParentId { get; set; }
    public ApplicationUser Parent { get; set; } = null!;

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public ICollection<ParentTestQuestion> ParentTestQuestions { get; set; } = [];
    public ICollection<ParentTestResult> ParentTestResults { get; set; } = [];
}
