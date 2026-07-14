using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.LiveSessions.Mappings;

public static class LiveSessionDtoBuilder
{
    public static LiveSessionDto Build(LiveSession session) => new()
    {
        Id = session.Id,
        Title = session.Title,
        Description = session.Description,
        CourseId = session.CourseId,
        CourseTitle = session.Course.Title,
        InstructorId = session.InstructorId,
        InstructorName = $"{session.Instructor.FirstName} {session.Instructor.LastName}",
        MeetingPlatform = session.MeetingPlatform.ToString(),
        MeetingLink = session.MeetingLink,
        MeetingPassword = session.MeetingPassword,
        StartDateTime = session.StartDateTime,
        EndDateTime = session.EndDateTime,
        Status = EffectiveStatus(session).ToString(),
        CreatedAt = session.CreatedAt
    };

    /// <summary>A Scheduled session whose EndDateTime has passed reads as Completed without
    /// needing a background job to persist the transition; an explicit Cancelled status is
    /// never overridden by the passage of time.</summary>
    public static LiveSessionStatus EffectiveStatus(LiveSession session) =>
        session.Status == LiveSessionStatus.Scheduled && session.EndDateTime <= DateTime.UtcNow
            ? LiveSessionStatus.Completed
            : session.Status;
}
