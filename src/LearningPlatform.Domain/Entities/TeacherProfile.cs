using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class TeacherProfile : BaseEntity
{
    public string? ProfileImageUrl { get; set; }
    public string RealName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public List<string> Subjects { get; set; } = [];
    public List<string> Grades { get; set; } = [];
    public string? Governorate { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? Bio { get; set; }
    public decimal? LessonPrice { get; set; }
    public List<string> Certificates { get; set; } = [];
    public List<string> Qualifications { get; set; } = [];
    public List<string> RequiredDocuments { get; set; } = [];

    // Verification
    public TeacherVerificationStatus VerificationStatus { get; set; } = TeacherVerificationStatus.UnderReview;
    public string? RejectionReason { get; set; }
    public bool AcceptingBookings { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public bool CanReceiveBookings =>
        VerificationStatus == TeacherVerificationStatus.Approved && AcceptingBookings;

    public ICollection<TeacherVerificationHistory> VerificationHistory { get; set; } = [];
}
