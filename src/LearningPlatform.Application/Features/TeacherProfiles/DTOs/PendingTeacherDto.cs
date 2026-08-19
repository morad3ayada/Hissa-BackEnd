using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.TeacherProfiles.DTOs;

public class PendingTeacherDto
{
    public Guid TeacherProfileId { get; set; }
    public Guid UserId { get; set; }
    public string RealName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Governorate { get; set; }
    public int? YearsOfExperience { get; set; }
    public decimal? LessonPrice { get; set; }
    public List<string> Subjects { get; set; } = [];
    public List<string> Grades { get; set; } = [];
    public List<string> Certificates { get; set; } = [];
    public List<string> Qualifications { get; set; } = [];
    public List<string> RequiredDocuments { get; set; } = [];
    public TeacherVerificationStatus VerificationStatus { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime SubmittedAt { get; set; }
}
