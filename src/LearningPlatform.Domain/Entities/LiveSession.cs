using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

/// <summary>A scheduled meeting hosted on an external platform (Zoom/Meet/Teams) — this
/// platform only stores and shares the link; it does not host the call itself.</summary>
public class LiveSession : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public LiveMeetingPlatform MeetingPlatform { get; set; }
    public string MeetingLink { get; set; } = string.Empty;
    public string? MeetingPassword { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public LiveSessionStatus Status { get; set; } = LiveSessionStatus.Scheduled;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public Guid InstructorId { get; set; }
    public ApplicationUser Instructor { get; set; } = null!;
}
