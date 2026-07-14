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

public class GetLiveSessionByIdQueryHandler(IUnitOfWork unitOfWork, ILiveSessionAccessService accessService)
    : IRequestHandler<GetLiveSessionByIdQuery, ApiResponse<LiveSessionDto>>
{
    public async Task<ApiResponse<LiveSessionDto>> Handle(GetLiveSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await unitOfWork.Repository<LiveSession>().AsQueryable()
            .Include(s => s.Course)
            .Include(s => s.Instructor)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(LiveSession), request.Id);

        await accessService.EnsureCanViewAsync(session, cancellationToken);

        var dto = LiveSessionDtoBuilder.Build(session);

        return ApiResponse<LiveSessionDto>.Success(dto);
    }
}
