using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Commands;

public record CreateNoteCommand : IRequest<ApiResponse<TeacherStudentNoteDto>>
{
    public Guid StudentId { get; init; }
    public string Note { get; init; } = string.Empty;
}
