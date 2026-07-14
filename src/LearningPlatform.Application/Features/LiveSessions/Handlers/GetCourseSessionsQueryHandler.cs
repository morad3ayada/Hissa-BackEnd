using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Application.Features.LiveSessions.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.Mappings;
using LearningPlatform.Application.Features.LiveSessions.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.LiveSessions.Handlers;

public class GetCourseSessionsQueryHandler(IUnitOfWork unitOfWork, ILiveSessionAccessService accessService)
    : IRequestHandler<GetCourseSessionsQuery, ApiResponse<List<LiveSessionDto>>>
{
    public async Task<ApiResponse<List<LiveSessionDto>>> Handle(GetCourseSessionsQuery request, CancellationToken cancellationToken)
    {
        var course = await unitOfWork.Repository<Course>().GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        await accessService.EnsureCanViewCourseSessionsAsync(course, cancellationToken);

        var sessions = await unitOfWork.Repository<LiveSession>().AsQueryable()
            .Include(s => s.Course)
            .Include(s => s.Instructor)
            .Where(s => s.CourseId == request.CourseId)
            .OrderByDescending(s => s.StartDateTime)
            .ToListAsync(cancellationToken);

        var dtos = sessions.Select(LiveSessionDtoBuilder.Build).ToList();

        return ApiResponse<List<LiveSessionDto>>.Success(dtos);
    }
}
