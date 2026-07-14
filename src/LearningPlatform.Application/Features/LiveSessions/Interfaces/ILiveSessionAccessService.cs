using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.LiveSessions.Interfaces;

public interface ILiveSessionAccessService
{
    /// <summary>Throws unless the current user may view this session: Admin, the owning
    /// instructor, or a student with an active enrollment in the session's course.</summary>
    Task EnsureCanViewAsync(LiveSession session, CancellationToken cancellationToken = default);

    /// <summary>Same rule as EnsureCanViewAsync, keyed by Course instead of a loaded session —
    /// used by GetCourseSessions before any specific session is known.</summary>
    Task EnsureCanViewCourseSessionsAsync(Course course, CancellationToken cancellationToken = default);

    /// <summary>Role-scoped session query: Admin sees everything, Instructor sees their own
    /// hosted sessions, Student sees sessions for courses they're actively enrolled in.</summary>
    Task<IQueryable<LiveSession>> GetVisibleSessionsQueryAsync(CancellationToken cancellationToken = default);
}
