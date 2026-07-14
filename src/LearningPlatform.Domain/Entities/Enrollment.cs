using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class Enrollment : BaseEntity
{
    public DateTime EnrolledAt { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; } = [];
    public ICollection<CourseProgress> CourseProgresses { get; set; } = [];
    public Certificate? Certificate { get; set; }
}
