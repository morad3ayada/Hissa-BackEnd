using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.Commands;
using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Helpers;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class CreateCourseCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    UserManager<ApplicationUser> userManager,
    IMapper mapper)
    : IRequestHandler<CreateCourseCommand, ApiResponse<CourseDto>>
{
    public async Task<ApiResponse<CourseDto>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var instructorId = currentUser.UserId!.Value;
        var instructor = await userManager.FindByIdAsync(instructorId.ToString())
            ?? throw new InvalidOperationException("Authenticated user could not be resolved.");

        var course = new Course
        {
            Title = request.Title,
            Slug = SlugGenerator.GenerateUniqueSlug(request.Title),
            Description = request.Description,
            Category = request.Category,
            Price = request.Price,
            DiscountPrice = request.DiscountPrice,
            Level = request.Level,
            Language = request.Language,
            Status = CourseStatus.Draft,
            InstructorId = instructorId,
            Instructor = instructor
        };

        await unitOfWork.Repository<Course>().AddAsync(course, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<CourseDto>.Success(mapper.Map<CourseDto>(course), "Course created successfully.");
    }
}
