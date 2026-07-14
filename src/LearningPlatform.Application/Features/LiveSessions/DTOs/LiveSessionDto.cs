namespace LearningPlatform.Application.Features.LiveSessions.DTOs;

public class LiveSessionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string MeetingPlatform { get; set; } = string.Empty;
    public string MeetingLink { get; set; } = string.Empty;
    public string? MeetingPassword { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
