using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Commands;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Handlers;

public class CreateQuizCommandHandler(
    IUnitOfWork unitOfWork,
    IQuizAuthorizationService quizAuthorization)
    : IRequestHandler<CreateQuizCommand, ApiResponse<QuizDto>>
{
    public async Task<ApiResponse<QuizDto>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = new Quiz
        {
            Title = request.Title,
            Scope = request.Scope,
            CourseId = request.CourseId,
            LessonId = request.LessonId,
            IsFinalExam = request.IsFinalExam,
            TimeLimitInMinutes = request.TimeLimitInMinutes,
            PassingScore = request.PassingScore,
            MaxAttempts = request.MaxAttempts,
            IsPublished = false
        };

        // Resolves the owning course from CourseId/LessonId and checks ownership in one step.
        await quizAuthorization.EnsureCanManageQuizAsync(quiz, cancellationToken);

        if (request.IsFinalExam)
        {
            var alreadyHasFinalExam = await unitOfWork.Repository<Quiz>().ExistsAsync(
                q => q.CourseId == request.CourseId && q.IsFinalExam, cancellationToken);

            if (alreadyHasFinalExam)
                throw new ConflictException("This course already has a final exam.");
        }

        await unitOfWork.Repository<Quiz>().AddAsync(quiz, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = QuizDtoBuilder.Build(quiz, includeAnswerKey: true);

        return ApiResponse<QuizDto>.Success(dto, "Quiz created successfully.");
    }
}
