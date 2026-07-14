using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Commands;
using LearningPlatform.Application.Features.Quizzes.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Handlers;

public class DeleteQuestionCommandHandler(IUnitOfWork unitOfWork, IQuizAuthorizationService quizAuthorization)
    : IRequestHandler<DeleteQuestionCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var questionRepository = unitOfWork.Repository<Question>();

        var question = await questionRepository.GetByIdAsync(request.QuestionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Question), request.QuestionId);

        var quiz = await unitOfWork.Repository<Quiz>().GetByIdAsync(question.QuizId, cancellationToken)
            ?? throw new NotFoundException(nameof(Quiz), question.QuizId);

        await quizAuthorization.EnsureCanManageQuizAsync(quiz, cancellationToken);

        // Never delete a question students have already answered — that would destroy
        // their submitted results and violate the "results are never deleted" rule.
        var hasBeenAnswered = await unitOfWork.Repository<StudentAnswer>().ExistsAsync(
            a => a.QuestionId == request.QuestionId, cancellationToken);

        if (hasBeenAnswered)
            throw new BadRequestException(
                "This question already has submitted student answers and cannot be deleted. Unpublish or edit it instead.");

        questionRepository.Remove(question);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Question deleted successfully.");
    }
}
