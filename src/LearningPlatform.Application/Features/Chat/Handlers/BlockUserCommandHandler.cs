using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class BlockUserCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<BlockUserCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        if (request.UserId == userId)
            throw new BadRequestException("You cannot block yourself.");

        var userToBlock = await userManager.FindByIdAsync(request.UserId.ToString())
            ?? throw new NotFoundException(nameof(ApplicationUser), request.UserId);

        var alreadyBlocked = await unitOfWork.Repository<BlockedUser>()
            .ExistsAsync(b => b.UserId == userId && b.BlockedUserId == request.UserId, cancellationToken);

        if (alreadyBlocked)
            return ApiResponse.Success("User is already blocked.");

        await unitOfWork.Repository<BlockedUser>().AddAsync(new BlockedUser
        {
            UserId = userId,
            BlockedUserId = request.UserId
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success($"User {userToBlock.FirstName} {userToBlock.LastName} has been blocked.");
    }
}
