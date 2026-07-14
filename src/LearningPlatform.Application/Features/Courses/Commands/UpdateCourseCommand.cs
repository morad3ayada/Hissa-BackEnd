using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Courses.Commands;

public record UpdateCourseCommand : IRequest<ApiResponse<CourseDto>>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Category { get; init; }
    public decimal Price { get; init; }
    public decimal? DiscountPrice { get; init; }
    public DifficultyLevel Level { get; init; }
    public string Language { get; init; } = "ar";
}
