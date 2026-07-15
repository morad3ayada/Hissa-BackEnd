using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Mappings;
using LearningPlatform.Application.Features.Quizzes.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Quizzes.Handlers;

public class GetQuizQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IQuizAuthorizationService quizAuthorization)
    : IRequestHandler<GetQuizQuery, ApiResponse<QuizDto>>
{
    public async Task<ApiResponse<QuizDto>> Handle(GetQuizQuery request, CancellationToken cancellationToken)
    {
        var quiz = await unitOfWork.Repository<Quiz>().AsQueryable()
            .Include(q => q.Questions.OrderBy(qs => qs.Order)).ThenInclude(qs => qs.Answers)
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Quiz), request.Id);

        var course = await quizAuthorization.GetQuizCourseAsync(quiz, cancellationToken);
        var isManager = currentUser.IsInRole(nameof(UserRole.Admin)) || currentUser.UserId == course.InstructorId;

        if (!isManager)
        {
            if (!quiz.IsPublished)
                throw new NotFoundException(nameof(Quiz), request.Id);

            await quizAuthorization.EnsureCanTakeQuizAsync(quiz, cancellationToken);

            var previousAttempts = await unitOfWork.Repository<QuizResult>()
                .FindAsync(r => r.QuizId == quiz.Id && r.StudentId == currentUser.UserId, cancellationToken);

            if (previousAttempts.Count >= (quiz.MaxAttempts ?? 1))
                throw new BadRequestException("You have already submitted this quiz and cannot enter it again.");
        }

        var dto = QuizDtoBuilder.Build(quiz, includeAnswerKey: isManager);

        return ApiResponse<QuizDto>.Success(dto);
    }
}
