using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Common.Services;

public class CourseDurationRecalculator(IUnitOfWork unitOfWork) : ICourseDurationRecalculator
{
    public async Task RecalculateAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var totalSeconds = await unitOfWork.Repository<Lesson>()
            .AsQueryable()
            .Where(l => l.CourseSection.CourseId == courseId)
            .SumAsync(l => l.DurationInSeconds ?? 0, cancellationToken);

        var courseRepository = unitOfWork.Repository<Course>();
        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);

        if (course is null)
            return;

        course.DurationInMinutes = totalSeconds > 0 ? (int)Math.Ceiling(totalSeconds / 60.0) : null;
        courseRepository.Update(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
