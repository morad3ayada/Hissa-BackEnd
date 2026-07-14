using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class DeleteCourseCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<DeleteCourseCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<Course>();

        var course = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.Id);

        currentUser.EnsureCanManageCourse(course);

        course.IsDeleted = true;
        repository.Update(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Course deleted successfully.");
    }
}
