using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Commands;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Handlers;

public class AddQuestionCommandHandler(IUnitOfWork unitOfWork, IQuizAuthorizationService quizAuthorization)
    : IRequestHandler<AddQuestionCommand, ApiResponse<QuestionDto>>
{
    public async Task<ApiResponse<QuestionDto>> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
    {
        var quiz = await unitOfWork.Repository<Quiz>().GetByIdAsync(request.QuizId, cancellationToken)
            ?? throw new NotFoundException(nameof(Quiz), request.QuizId);

        await quizAuthorization.EnsureCanManageQuizAsync(quiz, cancellationToken);

        var questionRepository = unitOfWork.Repository<Question>();
        var existingQuestions = await questionRepository.FindAsync(q => q.QuizId == request.QuizId, cancellationToken);
        var nextOrder = existingQuestions.Count == 0 ? 1 : existingQuestions.Max(q => q.Order) + 1;

        var question = new Question
        {
            QuizId = request.QuizId,
            Text = request.Text,
            Type = QuestionType.SingleChoice,
            Explanation = request.Explanation,
            Points = request.Points,
            Order = nextOrder
        };

        for (var i = 0; i < request.Answers.Count; i++)
        {
            question.Answers.Add(new Answer
            {
                Text = request.Answers[i].Text,
                IsCorrect = request.Answers[i].IsCorrect,
                Order = i + 1
            });
        }

        await questionRepository.AddAsync(question, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = QuizDtoBuilder.BuildQuestion(question, includeAnswerKey: true);

        return ApiResponse<QuestionDto>.Success(dto, "Question added successfully.");
    }
}
