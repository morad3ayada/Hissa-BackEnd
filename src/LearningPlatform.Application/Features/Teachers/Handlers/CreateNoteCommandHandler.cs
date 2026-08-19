using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class CreateNoteCommandHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Commands.CreateNoteCommand, ApiResponse<TeacherStudentNoteDto>>
{
    public async Task<ApiResponse<TeacherStudentNoteDto>> Handle(
        Commands.CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var hasRelationship = await unitOfWork.Repository<Booking>()
            .AsQueryable()
            .AnyAsync(b => b.TeacherId == userId && b.StudentId == request.StudentId
                && b.Status != BookingStatus.Cancelled, cancellationToken);

        if (!hasRelationship)
            throw new ForbiddenException("You do not have a relationship with this student.");

        var note = new TeacherStudentNote
        {
            TeacherId = userId,
            StudentId = request.StudentId,
            Note = request.Note
        };

        await unitOfWork.Repository<TeacherStudentNote>().AddAsync(note, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<TeacherStudentNoteDto>.Success(new TeacherStudentNoteDto
        {
            NoteId = note.Id,
            Note = note.Note,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        }, "Note created successfully.");
    }
}
