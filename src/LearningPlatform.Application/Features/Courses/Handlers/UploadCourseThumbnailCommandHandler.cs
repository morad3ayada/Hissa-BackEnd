using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class UploadCourseThumbnailCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IFileStorageService fileStorageService)
    : IRequestHandler<UploadCourseThumbnailCommand, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(UploadCourseThumbnailCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<Course>();

        var course = await repository.GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        currentUser.EnsureCanManageCourse(course);

        var previousThumbnail = course.ThumbnailUrl;
        var extension = Path.GetExtension(request.FileName);
        var relativePath = $"PublicUploads/Thumbnails/{course.Id}{extension}";

        var storedPath = await fileStorageService.UploadAsync(
            request.FileStream, relativePath, request.ContentType, cancellationToken);

        course.ThumbnailUrl = storedPath;
        repository.Update(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(previousThumbnail) && previousThumbnail != storedPath)
            await fileStorageService.DeleteAsync(previousThumbnail, cancellationToken);

        return ApiResponse<string>.Success(storedPath, "Thumbnail uploaded successfully.");
    }
}
