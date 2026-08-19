using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Teachers.Handlers;

public class GetStudentDetailQueryHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Queries.GetStudentDetailQuery, ApiResponse<StudentDetailDto>>
{
    public async Task<ApiResponse<StudentDetailDto>> Handle(
        Queries.GetStudentDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var hasRelationship = await unitOfWork.Repository<Booking>()
            .AsQueryable()
            .AnyAsync(b => b.TeacherId == userId && b.StudentId == request.StudentId, cancellationToken);

        if (!hasRelationship)
            throw new ForbiddenException("You do not have a relationship with this student.");

        var student = await unitOfWork.Repository<ApplicationUser>()
            .AsQueryable()
            .FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken)
            ?? throw new NotFoundException("Student not found.");

        var allBookings = await unitOfWork.Repository<Booking>()
            .AsQueryable()
            .Where(b => b.TeacherId == userId && b.StudentId == request.StudentId)
            .OrderByDescending(b => b.Date)
            .ToListAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var previousLessons = allBookings
            .Where(b => b.Date < today || b.Status == BookingStatus.Completed)
            .Select(b => new LessonHistoryDto
            {
                BookingId = b.Id,
                Subject = b.Subject,
                Date = b.Date,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                DurationInMinutes = b.DurationInMinutes,
                Price = b.Price,
                Status = b.Status
            })
            .ToList();

        var upcomingLessons = allBookings
            .Where(b => b.Date >= today && b.Status is BookingStatus.Pending or BookingStatus.Confirmed)
            .Select(b => new LessonHistoryDto
            {
                BookingId = b.Id,
                Subject = b.Subject,
                Date = b.Date,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                DurationInMinutes = b.DurationInMinutes,
                Price = b.Price,
                Status = b.Status
            })
            .ToList();

        var dto = new StudentDetailDto
        {
            StudentId = student.Id,
            Name = $"{student.FirstName} {student.LastName}",
            ImageUrl = student.ProfilePictureUrl,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            PreviousLessons = previousLessons,
            UpcomingLessons = upcomingLessons
        };

        return ApiResponse<StudentDetailDto>.Success(dto);
    }
}
