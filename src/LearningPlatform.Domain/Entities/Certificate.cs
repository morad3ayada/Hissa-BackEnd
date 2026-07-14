using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class Certificate : BaseEntity
{
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string? CertificateUrl { get; set; }

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public Guid? EnrollmentId { get; set; }
    public Enrollment? Enrollment { get; set; }
}
