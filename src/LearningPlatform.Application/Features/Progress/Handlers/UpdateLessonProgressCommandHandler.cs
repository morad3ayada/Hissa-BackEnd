using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Progress.Commands;
using LearningPlatform.Application.Features.Progress.DTOs;
using LearningPlatform.Application.Features.Progress.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Progress.Handlers;

public class UpdateLessonProgressCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IGamificationService gamificationService,
    ICourseProgressCalculator courseProgressCalculator,
    ICourseCompletionService courseCompletionService,
    IMapper mapper)
    : IRequestHandler<UpdateLessonProgressCommand, ApiResponse<LessonProgressDto>>
{
    private const decimal CompletionThreshold = 95m;
    private const int LessonCompletedPoints = 10;
    private const int CourseCompletedPoints = 50;
    private const int TenLessonsMilestone = 10;

    public async Task<ApiResponse<LessonProgressDto>> Handle(UpdateLessonProgressCommand request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var lesson = await unitOfWork.Repository<Lesson>().GetByIdAsync(request.LessonId, cancellationToken)
            ?? throw new NotFoundException(nameof(Lesson), request.LessonId);

        var section = await unitOfWork.Repository<CourseSection>().GetByIdAsync(lesson.CourseSectionId, cancellationToken)
            ?? throw new NotFoundException(nameof(CourseSection), lesson.CourseSectionId);

        var enrollment = (await unitOfWork.Repository<Enrollment>().FindAsync(
                e => e.CourseId == section.CourseId && e.StudentId == studentId, cancellationToken))
            .FirstOrDefault();

        if (enrollment is null || enrollment.Status != EnrollmentStatus.Active)
            throw new ForbiddenException("You must have an active, paid enrollment to track progress in this course.");

        var progressRepository = unitOfWork.Repository<CourseProgress>();
        var existing = (await progressRepository.FindAsync(
                p => p.EnrollmentId == enrollment.Id && p.LessonId == request.LessonId, cancellationToken))
            .FirstOrDefault();

        var clampedPercentage = Math.Clamp(request.WatchPercentage, 0, 100);
        var isNew = existing is null;

        existing ??= new CourseProgress
        {
            EnrollmentId = enrollment.Id,
            LessonId = request.LessonId,
            StudentId = studentId
        };

        // Monotonic: never let a lower/older report reduce what was already achieved.
        existing.WatchPercentage = Math.Max(existing.WatchPercentage, clampedPercentage);
        existing.CurrentSecond = request.CurrentSecond;
        existing.LastWatchedAt = DateTime.UtcNow;

        var isNewlyCompleted = false;
        if (!existing.IsCompleted && existing.WatchPercentage >= CompletionThreshold)
        {
            existing.IsCompleted = true;
            existing.CompletedAt = DateTime.UtcNow;
            isNewlyCompleted = true;
        }

        // A freshly AddAsync-tracked entity still holds a temporary generated key, so calling
        // Update() on it (which forces the Modified state) throws; only existing, already
        // fully-keyed rows need an explicit Update() call.
        if (isNew)
            await progressRepository.AddAsync(existing, cancellationToken);
        else
            progressRepository.Update(existing);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Gamification hooks run after the save above so the queries they issue (fresh,
        // AsNoTracking) see this lesson's completion as committed, not still-pending.
        if (isNewlyCompleted)
        {
            await gamificationService.AwardPointsAsync(
                studentId, LessonCompletedPoints, PointsReason.LessonCompleted, request.LessonId,
                cancellationToken: cancellationToken);

            var completedLessonsCount = await unitOfWork.Repository<CourseProgress>().AsQueryable()
                .CountAsync(p => p.StudentId == studentId && p.IsCompleted, cancellationToken);

            if (completedLessonsCount >= TenLessonsMilestone)
                await gamificationService.CheckAchievementAsync(studentId, AchievementTriggerType.TenLessonsCompleted, cancellationToken);

            var courseSummary = await courseProgressCalculator.CalculateSummaryAsync(section.CourseId, studentId, cancellationToken);
            if (courseSummary.CompletionPercentage >= 100)
            {
                await gamificationService.AwardPointsAsync(
                    studentId, CourseCompletedPoints, PointsReason.CourseCompleted, section.CourseId,
                    cancellationToken: cancellationToken);

                await gamificationService.CheckAchievementAsync(studentId, AchievementTriggerType.FirstCourseCompleted, cancellationToken);

                // Official completion (Certificate/PDF): no-ops if this course has a final exam
                // that hasn't been passed yet — that path is granted from SubmitQuizCommandHandler.
                await courseCompletionService.TryGrantCourseCompletionAsync(studentId, section.CourseId, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var result = await progressRepository.AsQueryable()
            .Include(p => p.Lesson)
            .FirstAsync(p => p.Id == existing.Id, cancellationToken);

        return ApiResponse<LessonProgressDto>.Success(mapper.Map<LessonProgressDto>(result), "Progress updated.");
    }
}
