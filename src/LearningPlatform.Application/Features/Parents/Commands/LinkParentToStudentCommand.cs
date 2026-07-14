using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Parents.Commands;

public record LinkParentToStudentCommand : IRequest<ApiResponse>
{
    public Guid ParentId { get; init; }
    public Guid StudentId { get; init; }
    public string? RelationshipType { get; init; }
}
