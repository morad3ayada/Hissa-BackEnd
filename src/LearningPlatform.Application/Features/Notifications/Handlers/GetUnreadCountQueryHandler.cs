using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Notifications.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Notifications.Handlers;

public class GetUnreadCountQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetUnreadCountQuery, ApiResponse<int>>
{
    public async Task<ApiResponse<int>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var count = await unitOfWork.Repository<Notification>().AsQueryable()
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

        return ApiResponse<int>.Success(count);
    }
}
