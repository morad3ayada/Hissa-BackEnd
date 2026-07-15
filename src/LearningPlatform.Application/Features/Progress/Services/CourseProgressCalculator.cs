using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Progress.DTOs;
using LearningPlatform.Application.Features.Progress.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Progress.Services;

public class CourseProgressCalculator(IUnitOfWork unitOfWork, IMapper mapper) : ICourseProgressCalculator
{
    public async Task<CourseProgressSummaryDto> CalculateSummaryAsync(
        Guid courseId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var (course, student, totalLessons, progresses) = await LoadAsync(courseId, studentId, cancellationToken);

        return BuildSummary(course, student, totalLessons, progresses);
    }

    public async Task<CourseProgressDetailDto> CalculateDetailAsync(
        Guid courseId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var (course, student, totalLessons, progresses) = await LoadAsync(courseId, studentId, cancellationToken);

        var summary = BuildSummary(course, student, totalLessons, progresses);

        return new CourseProgressDetailDto
        {
            CourseId = summary.CourseId,
            CourseTitle = summary.CourseTitle,
            CourseThumbnailUrl = summary.CourseThumbnailUrl,
            StudentId = summary.StudentId,
            StudentName = summary.StudentName,
            TotalLessons = summary.TotalLessons,
            CompletedLessons = summary.CompletedLessons,
            CompletionPercentage = summary.CompletionPercentage,
            OverallWatchPercentage = summary.OverallWatchPercentage,
            TotalWatchedSeconds = summary.TotalWatchedSeconds,
            LastWatchedAt = summary.LastWatchedAt,
            Lessons = mapper.Map<List<LessonProgressDto>>(progresses)
        };
    }

    private async Task<(Course Course, ApplicationUser Student, int TotalLessons, List<CourseProgress> Progresses)> LoadAsync(
        Guid courseId, Guid studentId, CancellationToken cancellationToken)
    {
        var course = await unitOfWork.Repository<Course>().GetByIdAsync(courseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), courseId);

        var totalLessons = await unitOfWork.Repository<Lesson>()
            .AsQueryable()
            .CountAsync(l => l.CourseSection.CourseId == courseId, cancellationToken);

        var progresses = await unitOfWork.Repository<CourseProgress>()
            .AsQueryable()
            .Include(p => p.Lesson)
            .Where(p => p.StudentId == studentId && p.Lesson.CourseSection.CourseId == courseId)
            .ToListAsync(cancellationToken);

        // A lightweight ApplicationUser stand-in for name display; full identity lookups
        // (roles, etc.) aren't needed here, so the repository's own Instructor include is
        // reused only for Course; the student is fetched via the progress/enrollment side.
        var student = await unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Include(e => e.Student)
            .Where(e => e.CourseId == courseId && e.StudentId == studentId)
            .Select(e => e.Student)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("This student is not enrolled in this course.");

        return (course, student, totalLessons, progresses);
    }

    private static CourseProgressSummaryDto BuildSummary(
        Course course, ApplicationUser student, int totalLessons, List<CourseProgress> progresses)
    {
        var completedLessons = progresses.Count(p => p.IsCompleted);

        return new CourseProgressSummaryDto
        {
            CourseId = course.Id,
            CourseTitle = course.Title,
            CourseThumbnailUrl = course.ThumbnailUrl,
            StudentId = student.Id,
            StudentName = $"{student.FirstName} {student.LastName}",
            TotalLessons = totalLessons,
            CompletedLessons = completedLessons,
            // Strict "how many lessons are fully done" ratio — this is what gamification/
            // certificate logic keys off of (>=100 means every lesson individually crossed
            // the completion threshold), so it must stay a completed-lesson count, not an
            // average of in-progress watch percentages.
            CompletionPercentage = totalLessons == 0
                ? 0
                : Math.Round(completedLessons * 100m / totalLessons, 2),
            // Smoother "how much of the course have you engaged with" figure for progress
            // bars — includes partial watch progress on not-yet-completed lessons, so it
            // isn't stuck at 0% until a lesson crosses the 95% completion threshold.
            OverallWatchPercentage = totalLessons == 0
                ? 0
                : Math.Round(progresses.Sum(p => p.WatchPercentage) / totalLessons, 2),
            TotalWatchedSeconds = progresses.Sum(p => p.CurrentSecond),
            LastWatchedAt = progresses.Count == 0 ? null : progresses.Max(p => p.LastWatchedAt)
        };
    }
}
