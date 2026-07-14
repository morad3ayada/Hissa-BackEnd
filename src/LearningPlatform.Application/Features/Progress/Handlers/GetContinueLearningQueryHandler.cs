using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Progress.DTOs;
using LearningPlatform.Application.Features.Progress.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Progress.Handlers;

public class GetContinueLearningQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetContinueLearningQuery, ApiResponse<List<ContinueLearningDto>>>
{
    public async Task<ApiResponse<List<ContinueLearningDto>>> Handle(GetContinueLearningQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var activeCourseIds = await unitOfWork.Repository<Enrollment>()
            .AsQueryable()
            .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Active)
            .Select(e => e.CourseId)
            .ToListAsync(cancellationToken);

        var progressRepository = unitOfWork.Repository<CourseProgress>();
        var result = new List<ContinueLearningDto>();

        foreach (var courseId in activeCourseIds)
        {
            var lastWatched = await progressRepository
                .AsQueryable()
                .Include(p => p.Lesson).ThenInclude(l => l.CourseSection).ThenInclude(s => s.Course)
                .Where(p => p.StudentId == studentId && p.Lesson.CourseSection.CourseId == courseId && p.LastWatchedAt != null)
                .OrderByDescending(p => p.LastWatchedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastWatched is null)
                continue;

            result.Add(new ContinueLearningDto
            {
                CourseId = courseId,
                CourseTitle = lastWatched.Lesson.CourseSection.Course.Title,
                CourseThumbnailUrl = lastWatched.Lesson.CourseSection.Course.ThumbnailUrl,
                LessonId = lastWatched.LessonId,
                LessonTitle = lastWatched.Lesson.Title,
                SectionId = lastWatched.Lesson.CourseSectionId,
                SectionTitle = lastWatched.Lesson.CourseSection.Title,
                CurrentSecond = lastWatched.CurrentSecond,
                WatchPercentage = lastWatched.WatchPercentage,
                LastWatchedAt = lastWatched.LastWatchedAt!.Value
            });
        }

        return ApiResponse<List<ContinueLearningDto>>.Success(
            result.OrderByDescending(r => r.LastWatchedAt).ToList());
    }
}
