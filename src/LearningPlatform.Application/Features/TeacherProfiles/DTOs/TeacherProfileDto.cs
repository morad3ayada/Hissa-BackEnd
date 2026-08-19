using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.TeacherProfiles.DTOs;

public class TeacherProfileDto : IMapFrom<TeacherProfile>
{
    public Guid Id { get; set; }
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
    public TeacherVerificationStatus VerificationStatus { get; set; }
    public string? RejectionReason { get; set; }
    public bool AcceptingBookings { get; set; }
    public bool CanReceiveBookings { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
