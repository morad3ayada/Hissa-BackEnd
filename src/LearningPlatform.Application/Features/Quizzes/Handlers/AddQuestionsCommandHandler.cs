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

public class AddQuestionsCommandHandler(IUnitOfWork unitOfWork, IQuizAuthorizationService quizAuthorization)
    : IRequestHandler<AddQuestionsCommand, ApiResponse<List<QuestionDto>>>
{
    public async Task<ApiResponse<List<QuestionDto>>> Handle(
        AddQuestionsCommand request,
        CancellationToken cancellationToken)
    {
        var quiz = await unitOfWork.Repository<Quiz>().GetByIdAsync(request.QuizId, cancellationToken)
            ?? throw new NotFoundException(nameof(Quiz), request.QuizId);

        await quizAuthorization.EnsureCanManageQuizAsync(quiz, cancellationToken);

        var questionRepository = unitOfWork.Repository<Question>();

        // نحسب الـ order الحالي مرة واحدة بس قبل الحلقة
        var existingQuestions = await questionRepository.FindAsync(q => q.QuizId == request.QuizId, cancellationToken);
        var nextOrder = existingQuestions.Count == 0 ? 1 : existingQuestions.Max(q => q.Order) + 1;

        var addedQuestions = new List<Question>();

        for (var i = 0; i < request.Questions.Count; i++)
        {
            var input = request.Questions[i];

            // نحول الـ Type string إلى enum
            var questionType = Enum.TryParse<QuestionType>(input.Type, ignoreCase: true, out var parsed)
                ? parsed
                : QuestionType.SingleChoice;

            var question = new Question
            {
                QuizId      = request.QuizId,
                Text        = input.Text,
                Type        = questionType,
                Explanation = input.Explanation,
                Points      = input.Points,
                Order       = nextOrder + i
            };

            for (var j = 0; j < input.Answers.Count; j++)
            {
                question.Answers.Add(new Answer
                {
                    Text      = input.Answers[j].Text,
                    IsCorrect = input.Answers[j].IsCorrect,
                    Order     = j + 1
                });
            }

            await questionRepository.AddAsync(question, cancellationToken);
            addedQuestions.Add(question);
        }

        // SaveChanges مرة واحدة للكل — أكفأ من SaveChanges داخل الحلقة
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dtos = addedQuestions
            .Select(q => QuizDtoBuilder.BuildQuestion(q, includeAnswerKey: true))
            .ToList();

        return ApiResponse<List<QuestionDto>>.Success(
            dtos,
            $"{dtos.Count} question(s) added successfully.");
    }
}
