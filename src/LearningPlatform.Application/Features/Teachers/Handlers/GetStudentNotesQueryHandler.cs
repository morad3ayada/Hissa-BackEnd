using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class GetStudentNotesQueryHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Queries.GetStudentNotesQuery, ApiResponse<List<TeacherStudentNoteDto>>>
{
    public async Task<ApiResponse<List<TeacherStudentNoteDto>>> Handle(
        Queries.GetStudentNotesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var notes = await unitOfWork.Repository<TeacherStudentNote>()
            .AsQueryable()
            .Where(n => n.TeacherId == userId && n.StudentId == request.StudentId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new TeacherStudentNoteDto
            {
                NoteId = n.Id,
                Note = n.Note,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<TeacherStudentNoteDto>>.Success(notes);
    }
}
