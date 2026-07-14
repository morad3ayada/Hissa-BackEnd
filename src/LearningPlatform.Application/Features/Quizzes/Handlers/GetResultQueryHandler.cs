using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Mappings;
using LearningPlatform.Application.Features.Quizzes.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Quizzes.Handlers;

public class GetResultQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IQuizAuthorizationService quizAuthorization)
    : IRequestHandler<GetResultQuery, ApiResponse<QuizResultDto>>
{
    public async Task<ApiResponse<QuizResultDto>> Handle(GetResultQuery request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Repository<QuizResult>().AsQueryable()
            .Include(r => r.Quiz)
            .Include(r => r.StudentAnswers).ThenInclude(a => a.Question).ThenInclude(q => q.Answers)
            .Include(r => r.StudentAnswers).ThenInclude(a => a.SelectedAnswer)
            .FirstOrDefaultAsync(r => r.Id == request.AttemptId, cancellationToken)
            ?? throw new NotFoundException(nameof(QuizResult), request.AttemptId);

        if (result.StudentId != currentUser.UserId)
            await quizAuthorization.EnsureCanManageQuizAsync(result.Quiz, cancellationToken);

        var dto = QuizDtoBuilder.BuildResult(result, result.Quiz.Title);

        return ApiResponse<QuizResultDto>.Success(dto);
    }
}
