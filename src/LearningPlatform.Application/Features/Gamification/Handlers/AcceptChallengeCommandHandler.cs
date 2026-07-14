using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Gamification.Commands;
using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Application.Features.Gamification.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Gamification.Handlers;

public class AcceptChallengeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<AcceptChallengeCommand, ApiResponse<ChallengeDto>>
{
    public async Task<ApiResponse<ChallengeDto>> Handle(AcceptChallengeCommand request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var challenge = await unitOfWork.Repository<Challenge>().AsQueryable()
            .Include(c => c.Challenger)
            .Include(c => c.Opponent)
            .Include(c => c.Quiz)
            .FirstOrDefaultAsync(c => c.Id == request.ChallengeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Challenge), request.ChallengeId);

        if (challenge.OpponentId != studentId)
            throw new ForbiddenException("Only the invited opponent can accept this challenge.");

        if (challenge.Status != ChallengeStatus.NotStarted)
            throw new BadRequestException("This challenge cannot be accepted (already accepted, completed, or expired).");

        challenge.Status = ChallengeStatus.InProgress;
        challenge.AcceptedAt = DateTime.UtcNow;

        unitOfWork.Repository<Challenge>().Update(challenge);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = ChallengeDtoBuilder.Build(challenge, myScore: null, opponentScore: null);

        return ApiResponse<ChallengeDto>.Success(dto, "Challenge accepted.");
    }
}
