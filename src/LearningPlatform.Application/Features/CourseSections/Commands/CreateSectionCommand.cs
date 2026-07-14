using LearningPlatform.Application.Features.CourseSections.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.CourseSections.Commands;

public record CreateSectionCommand : IRequest<ApiResponse<SectionDto>>
{
    public Guid CourseId { get; init; }
    public string Title { get; init; } = string.Empty;
}
