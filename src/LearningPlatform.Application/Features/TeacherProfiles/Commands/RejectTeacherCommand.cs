using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.TeacherProfiles.Commands;

public record RejectTeacherCommand : IRequest<ApiResponse>
{
    public Guid TeacherProfileId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
