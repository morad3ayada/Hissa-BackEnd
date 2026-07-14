using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.Commands;
using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class SetCourseStateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
    : IRequestHandler<SetCourseStateCommand, ApiResponse<CourseDto>>
{
    public async Task<ApiResponse<CourseDto>> Handle(SetCourseStateCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<Course>();

        var course = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.Id);

        course.Status = request.Status;

        if (request.Status == CourseStatus.Published && course.PublishedAt is null)
            course.PublishedAt = DateTime.UtcNow;

        repository.Update(course);

        if (request.Status is CourseStatus.Published or CourseStatus.Rejected)
        {
            await notificationService.CreateAsync(
                course.InstructorId, NotificationType.Course,
                request.Status == CourseStatus.Published ? "Course published" : "Course rejected",
                request.Status == CourseStatus.Published
                    ? $"Your course \"{course.Title}\" has been published."
                    : $"Your course \"{course.Title}\" was rejected.",
                cancellationToken: cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await repository.AsQueryable()
            .Include(c => c.Instructor)
            .Include(c => c.CourseSections)
            .FirstAsync(c => c.Id == course.Id, cancellationToken);

        return ApiResponse<CourseDto>.Success(mapper.Map<CourseDto>(result), $"Course status changed to {request.Status}.");
    }
}
