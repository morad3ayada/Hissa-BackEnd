using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.LiveSessions.Handlers;

public class DeleteLiveSessionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<DeleteLiveSessionCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteLiveSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await unitOfWork.Repository<LiveSession>().AsQueryable()
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(LiveSession), request.Id);

        currentUser.EnsureCanManageCourse(session.Course);

        unitOfWork.Repository<LiveSession>().Remove(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Live session deleted successfully.");
    }
}
