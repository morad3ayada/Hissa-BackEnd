using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Commands;

public record DeleteNoteCommand(Guid NoteId) : IRequest<ApiResponse>;
