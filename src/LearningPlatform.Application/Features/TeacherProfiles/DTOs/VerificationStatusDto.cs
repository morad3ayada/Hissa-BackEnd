using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.TeacherProfiles.DTOs;

public class VerificationStatusDto
{
    public TeacherVerificationStatus Status { get; set; }
    public bool IsVerified => Status == TeacherVerificationStatus.Approved;
    public bool CanReceiveBookings { get; set; }
    public string? RejectionReason { get; set; }
    public bool AcceptingBookings { get; set; }
}
