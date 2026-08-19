using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class GetTeacherStudentsQueryHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Queries.GetTeacherStudentsQuery, ApiResponse<List<TeacherStudentDto>>>
{
    public async Task<ApiResponse<List<TeacherStudentDto>>> Handle(
        Queries.GetTeacherStudentsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var students = await unitOfWork.Repository<Booking>()
            .AsQueryable()
            .Include(b => b.Student)
            .Where(b => b.TeacherId == userId
                && b.Status != BookingStatus.Cancelled)
            .GroupBy(b => b.StudentId)
            .Select(g => new TeacherStudentDto
            {
                StudentId = g.Key,
                Name = $"{g.First().Student.FirstName} {g.First().Student.LastName}",
                ImageUrl = g.First().Student.ProfilePictureUrl,
                LessonsCount = g.Count(),
                Subjects = g.Select(b => b.Subject).Distinct().ToList()
            })
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<TeacherStudentDto>>.Success(students);
    }
}
