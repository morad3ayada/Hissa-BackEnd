using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class UpdateNoteCommandHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Commands.UpdateNoteCommand, ApiResponse<TeacherStudentNoteDto>>
{
    public async Task<ApiResponse<TeacherStudentNoteDto>> Handle(
        Commands.UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var note = await unitOfWork.Repository<TeacherStudentNote>()
            .GetByIdAsync(request.NoteId, cancellationToken)
            ?? throw new NotFoundException("Note not found.");

        if (note.TeacherId != userId)
            throw new ForbiddenException("You can only edit your own notes.");

        note.Note = request.Note;
        unitOfWork.Repository<TeacherStudentNote>().Update(note);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<TeacherStudentNoteDto>.Success(new TeacherStudentNoteDto
        {
            NoteId = note.Id,
            Note = note.Note,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        }, "Note updated successfully.");
    }
}
