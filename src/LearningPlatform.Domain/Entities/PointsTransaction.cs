using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

/// <summary>Audit trail of every points grant, and the idempotency source of truth: a
/// (StudentId, Reason, SourceId) row can only exist once, so the same lesson/course/quiz/
/// challenge/reward can never grant points twice.</summary>
public class PointsTransaction : BaseEntity
{
    public int Points { get; set; }
    public PointsReason Reason { get; set; }
    public Guid? SourceId { get; set; }
    public string? Notes { get; set; }

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;
}
