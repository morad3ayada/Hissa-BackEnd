using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Application.Features.Courses.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Courses.Handlers;

public class GetCourseByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetCourseByIdQuery, ApiResponse<CourseDto>>
{
    public async Task<ApiResponse<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await unitOfWork.Repository<Course>()
            .AsQueryable()
            .Include(c => c.Instructor)
            .Include(c => c.CourseSections)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.Id);

        return ApiResponse<CourseDto>.Success(mapper.Map<CourseDto>(course));
    }
}