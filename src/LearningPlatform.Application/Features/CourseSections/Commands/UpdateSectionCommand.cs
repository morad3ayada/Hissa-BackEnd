using LearningPlatform.Application.Features.CourseSections.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.CourseSections.Commands;

public record UpdateSectionCommand : IRequest<ApiResponse<SectionDto>>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Order { get; init; }
}
