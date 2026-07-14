using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Courses.Commands;

public record CreateCourseCommand : IRequest<ApiResponse<CourseDto>>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Category { get; init; }
    public decimal Price { get; init; }
    public decimal? DiscountPrice { get; init; }
    public DifficultyLevel Level { get; init; } = DifficultyLevel.Beginner;
    public string Language { get; init; } = "ar";
}
