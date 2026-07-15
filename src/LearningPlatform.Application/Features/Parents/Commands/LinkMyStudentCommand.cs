using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Parents.Commands;

public record LinkMyStudentCommand : IRequest<ApiResponse>
{
    public string StudentEmail { get; init; } = string.Empty;
    public string? RelationshipType { get; init; }
}
