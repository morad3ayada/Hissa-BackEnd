using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Commands;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Quizzes.Handlers;

public class UpdateQuestionCommandHandler(IUnitOfWork unitOfWork, IQuizAuthorizationService quizAuthorization)
    : IRequestHandler<UpdateQuestionCommand, ApiResponse<QuestionDto>>
{
    public async Task<ApiResponse<QuestionDto>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await unitOfWork.Repository<Question>()
            .AsQueryable()
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Question), request.QuestionId);

        var quiz = await unitOfWork.Repository<Quiz>().GetByIdAsync(question.QuizId, cancellationToken)
            ?? throw new NotFoundException(nameof(Quiz), question.QuizId);

        await quizAuthorization.EnsureCanManageQuizAsync(quiz, cancellationToken);

        var existingAnswerIds = question.Answers.Select(a => a.Id).ToHashSet();
        var requestAnswerIds = request.Answers.Select(a => a.Id).ToHashSet();

        if (!existingAnswerIds.SetEquals(requestAnswerIds))
            throw new BadRequestException("The submitted answer IDs must exactly match this question's 4 existing answers.");

        question.Text = request.Text;
        question.Explanation = request.Explanation;
        question.Points = request.Points;

        foreach (var answerInput in request.Answers)
        {
            var answer = question.Answers.First(a => a.Id == answerInput.Id);
            answer.Text = answerInput.Text;
            answer.IsCorrect = answerInput.IsCorrect;
        }

        unitOfWork.Repository<Question>().Update(question);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = QuizDtoBuilder.BuildQuestion(question, includeAnswerKey: true);

        return ApiResponse<QuestionDto>.Success(dto, "Question updated successfully.");
    }
}
