using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.Commands;
using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Application.Features.LiveSessions.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.LiveSessions.Handlers;

public class CreateLiveSessionCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    INotificationService notificationService)
    : IRequestHandler<CreateLiveSessionCommand, ApiResponse<LiveSessionDto>>
{
    public async Task<ApiResponse<LiveSessionDto>> Handle(CreateLiveSessionCommand request, CancellationToken cancellationToken)
    {
        var course = await unitOfWork.Repository<Course>().GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        currentUser.EnsureCanManageCourse(course);

        // The session's host is always the course's own instructor, regardless of whether an
        // Admin is the one calling Create on their behalf.
        var hasOverlap = await unitOfWork.Repository<LiveSession>().ExistsAsync(
            s => s.InstructorId == course.InstructorId &&
                 s.Status != LiveSessionStatus.Cancelled &&
                 s.StartDateTime < request.EndDateTime &&
                 request.StartDateTime < s.EndDateTime,
            cancellationToken);

        if (hasOverlap)
            throw new ConflictException("This instructor already has another live session scheduled during this time.");

        var session = new LiveSession
        {
            Title = request.Title,
            Description = request.Description,
            CourseId = request.CourseId,
            InstructorId = course.InstructorId,
            MeetingPlatform = request.MeetingPlatform,
            MeetingLink = request.MeetingLink,
            MeetingPassword = request.MeetingPassword,
            StartDateTime = request.StartDateTime,
            EndDateTime = request.EndDateTime,
            Status = LiveSessionStatus.Scheduled
        };

        await unitOfWork.Repository<LiveSession>().AddAsync(session, cancellationToken);

        var enrolledStudentIds = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .Where(e => e.CourseId == course.Id && e.Status == EnrollmentStatus.Active)
            .Select(e => e.StudentId)
            .ToListAsync(cancellationToken);

        foreach (var studentId in enrolledStudentIds)
        {
            await notificationService.CreateAsync(
                studentId, NotificationType.Live, "New live session scheduled",
                $"A new live session \"{session.Title}\" was scheduled in \"{course.Title}\" for {session.StartDateTime:g} UTC.",
                cancellationToken: cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var loaded = await unitOfWork.Repository<LiveSession>().AsQueryable()
            .Include(s => s.Course)
            .Include(s => s.Instructor)
            .FirstAsync(s => s.Id == session.Id, cancellationToken);

        var dto = LiveSessionDtoBuilder.Build(loaded);

        return ApiResponse<LiveSessionDto>.Success(dto, "Live session created successfully.");
    }
}
