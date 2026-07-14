using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Gamification.Commands;
using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Application.Features.Gamification.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Gamification.Handlers;

public class CreateChallengeCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<CreateChallengeCommand, ApiResponse<ChallengeDto>>
{
    private const int DefaultPointsReward = 50;

    public async Task<ApiResponse<ChallengeDto>> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        var challengerId = currentUser.UserId!.Value;

        if (request.OpponentId == challengerId)
            throw new BadRequestException("You can't challenge yourself.");

        var opponent = await userManager.FindByIdAsync(request.OpponentId.ToString())
            ?? throw new NotFoundException(nameof(ApplicationUser), request.OpponentId);

        if (opponent.Role != UserRole.Student)
            throw new BadRequestException("You can only challenge another student.");

        var quiz = await unitOfWork.Repository<Quiz>().GetByIdAsync(request.QuizId, cancellationToken)
            ?? throw new NotFoundException(nameof(Quiz), request.QuizId);

        if (!quiz.IsPublished)
            throw new BadRequestException("This quiz is not published yet.");

        var challenge = new Challenge
        {
            Title = string.IsNullOrWhiteSpace(request.Title) ? "Quiz Challenge" : request.Title,
            ChallengerId = challengerId,
            OpponentId = request.OpponentId,
            QuizId = request.QuizId,
            DurationInMinutes = request.DurationInMinutes,
            PointsReward = DefaultPointsReward,
            Status = ChallengeStatus.NotStarted
        };

        challenge.StudentChallenges.Add(new StudentChallenge { StudentId = challengerId, Status = ChallengeStatus.NotStarted });
        challenge.StudentChallenges.Add(new StudentChallenge { StudentId = request.OpponentId, Status = ChallengeStatus.NotStarted });

        await unitOfWork.Repository<Challenge>().AddAsync(challenge, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var loaded = await unitOfWork.Repository<Challenge>().AsQueryable()
            .Include(c => c.Challenger)
            .Include(c => c.Opponent)
            .Include(c => c.Quiz)
            .FirstAsync(c => c.Id == challenge.Id, cancellationToken);

        var dto = ChallengeDtoBuilder.Build(loaded, myScore: null, opponentScore: null);

        return ApiResponse<ChallengeDto>.Success(dto, "Challenge sent.");
    }
}
