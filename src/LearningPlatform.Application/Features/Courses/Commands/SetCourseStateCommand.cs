using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Courses.Commands;

public record SetCourseStateCommand : IRequest<ApiResponse<CourseDto>>
{
    public Guid Id { get; init; }
    public CourseStatus Status { get; init; }
}
