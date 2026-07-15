namespace LearningPlatform.Application.Features.LiveSessions.DTOs;

public class LiveSessionAttendanceDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public DateTime JoinedAt { get; set; }
}
