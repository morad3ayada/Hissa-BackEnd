using LearningPlatform.Application.Features.Instructors.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Instructors.Queries;

public record GetInstructorsQuery : IRequest<PaginatedResponse<InstructorDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? Search { get; init; }
}
