using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class UnblockUserCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<UnblockUserCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var block = await unitOfWork.Repository<BlockedUser>()
            .GetTrackedAsync(b => b.UserId == userId && b.BlockedUserId == request.UserId, cancellationToken);

        if (block is null)
            return ApiResponse.Success("User is not blocked.");

        unitOfWork.Repository<BlockedUser>().Remove(block);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("User has been unblocked.");
    }
}
