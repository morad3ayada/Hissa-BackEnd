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
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Quizzes.Handlers;

public class PublishQuizCommandHandler(
    IUnitOfWork unitOfWork,
    IQuizAuthorizationService quizAuthorization,
    INotificationService notificationService)
    : IRequestHandler<PublishQuizCommand, ApiResponse<QuizDto>>
{
    public async Task<ApiResponse<QuizDto>> Handle(PublishQuizCommand request, CancellationToken cancellationToken)
    {
        var quizRepository = unitOfWork.Repository<Quiz>();

        var quiz = await quizRepository.AsQueryable()
            .Include(q => q.Questions).ThenInclude(qs => qs.Answers)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken)
            ?? throw new NotFoundException(nameof(Quiz), request.QuizId);

        var course = await quizAuthorization.EnsureCanManageQuizAsync(quiz, cancellationToken);

        if (quiz.Questions.Count == 0)
            throw new BadRequestException("Add at least one question before publishing this quiz.");

        quiz.IsPublished = true;
        quizRepository.Update(quiz);

        var enrolledStudentIds = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .Where(e => e.CourseId == course.Id && e.Status == EnrollmentStatus.Active)
            .Select(e => e.StudentId)
            .ToListAsync(cancellationToken);

        foreach (var studentId in enrolledStudentIds)
        {
            await notificationService.CreateAsync(
                studentId, NotificationType.Course, "New quiz available",
                $"A new quiz \"{quiz.Title}\" is now available in \"{course.Title}\".",
                cancellationToken: cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = QuizDtoBuilder.Build(quiz, includeAnswerKey: true);

        return ApiResponse<QuizDto>.Success(dto, "Quiz published successfully.");
    }
}
