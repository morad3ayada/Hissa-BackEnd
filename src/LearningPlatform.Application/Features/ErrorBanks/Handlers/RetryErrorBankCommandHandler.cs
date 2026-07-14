using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.ErrorBanks.Commands;
using LearningPlatform.Application.Features.ErrorBanks.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.ErrorBanks.Handlers;

public class RetryErrorBankCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<RetryErrorBankCommand, ApiResponse<List<RetryResultDto>>>
{
    public async Task<ApiResponse<List<RetryResultDto>>> Handle(RetryErrorBankCommand request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;
        var questionIds = request.Answers.Select(a => a.QuestionId).ToList();

        var errorEntries = await unitOfWork.Repository<ErrorBank>().AsQueryable()
            .Include(e => e.Question).ThenInclude(q => q.Answers)
            .Where(e => e.StudentId == studentId && questionIds.Contains(e.QuestionId))
            .ToListAsync(cancellationToken);

        if (errorEntries.Count != questionIds.Distinct().Count())
            throw new BadRequestException("One or more questions are not in your error bank.");

        var results = new List<RetryResultDto>();

        foreach (var answerInput in request.Answers)
        {
            var entry = errorEntries.First(e => e.QuestionId == answerInput.QuestionId);
            var correctAnswer = entry.Question.Answers.FirstOrDefault(a => a.IsCorrect);
            var isCorrect = answerInput.SelectedAnswerId.HasValue && answerInput.SelectedAnswerId.Value == correctAnswer?.Id;

            if (isCorrect)
            {
                entry.IsResolved = true;
                entry.ResolvedAt = DateTime.UtcNow;
            }
            else
            {
                entry.MistakeCount++;
                entry.LastMistakeAt = DateTime.UtcNow;
                entry.IsResolved = false;
                entry.ResolvedAt = null;
            }

            unitOfWork.Repository<ErrorBank>().Update(entry);

            results.Add(new RetryResultDto
            {
                QuestionId = entry.QuestionId,
                IsCorrect = isCorrect,
                IsResolved = entry.IsResolved,
                CorrectAnswerId = correctAnswer?.Id,
                CorrectAnswerText = correctAnswer?.Text,
                Explanation = entry.Question.Explanation
            });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<List<RetryResultDto>>.Success(results);
    }
}
