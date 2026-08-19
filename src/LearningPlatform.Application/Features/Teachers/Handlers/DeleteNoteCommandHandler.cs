using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class DeleteNoteCommandHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Commands.DeleteNoteCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(
        Commands.DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var note = await unitOfWork.Repository<TeacherStudentNote>()
            .GetByIdAsync(request.NoteId, cancellationToken)
            ?? throw new NotFoundException("Note not found.");

        if (note.TeacherId != userId)
            throw new ForbiddenException("You can only delete your own notes.");

        unitOfWork.Repository<TeacherStudentNote>().Delete(note);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Note deleted successfully.");
    }
}
