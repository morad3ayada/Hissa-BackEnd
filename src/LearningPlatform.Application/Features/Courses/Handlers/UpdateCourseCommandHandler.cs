using AutoMapper;
using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.Commands;
using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Helpers;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class UpdateCourseCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<UpdateCourseCommand, ApiResponse<CourseDto>>
{
    public async Task<ApiResponse<CourseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<Course>();

        var course = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.Id);

        currentUser.EnsureCanManageCourse(course);

        if (!string.Equals(course.Title, request.Title, StringComparison.Ordinal))
            course.Slug = SlugGenerator.GenerateUniqueSlug(request.Title);

        course.Title = request.Title;
        course.Description = request.Description;
        course.Category = request.Category;
        course.Price = request.Price;
        course.DiscountPrice = request.DiscountPrice;
        course.Level = request.Level;
        course.Language = request.Language;

        repository.Update(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await repository.AsQueryable()
            .Include(c => c.Instructor)
            .Include(c => c.CourseSections)
            .FirstAsync(c => c.Id == course.Id, cancellationToken);

        return ApiResponse<CourseDto>.Success(mapper.Map<CourseDto>(result), "Course updated successfully.");
    }
}
