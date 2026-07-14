using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.CourseSections.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.CourseSections.Handlers;

public class DeleteSectionCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    ICourseDurationRecalculator durationRecalculator)
    : IRequestHandler<DeleteSectionCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        var sectionRepository = unitOfWork.Repository<CourseSection>();

        var section = await sectionRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(CourseSection), request.Id);

        var course = await unitOfWork.Repository<Course>().GetByIdAsync(section.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), section.CourseId);

        currentUser.EnsureCanManageCourse(course);

        sectionRepository.Remove(section);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await durationRecalculator.RecalculateAsync(course.Id, cancellationToken);

        return ApiResponse.Success("Section deleted successfully.");
    }
}
