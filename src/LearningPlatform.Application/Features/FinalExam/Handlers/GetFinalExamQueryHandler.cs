using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.FinalExam.Queries;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.FinalExam.Handlers;

public class GetFinalExamQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IQuizAuthorizationService quizAuthorization)
    : IRequestHandler<GetFinalExamQuery, ApiResponse<QuizDto>>
{
    public async Task<ApiResponse<QuizDto>> Handle(GetFinalExamQuery request, CancellationToken cancellationToken)
    {
        var course = await unitOfWork.Repository<Course>().GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        var quiz = await unitOfWork.Repository<Quiz>().AsQueryable()
            .Include(q => q.Questions.OrderBy(qs => qs.Order)).ThenInclude(qs => qs.Answers)
            .FirstOrDefaultAsync(q => q.CourseId == request.CourseId && q.IsFinalExam, cancellationToken)
            ?? throw new NotFoundException("This course does not have a final exam yet.");

        var isManager = currentUser.IsInRole(nameof(UserRole.Admin)) || currentUser.UserId == course.InstructorId;

        if (!isManager)
        {
            if (!quiz.IsPublished)
                throw new NotFoundException("This course does not have a final exam yet.");

            await quizAuthorization.EnsureCanTakeQuizAsync(quiz, cancellationToken);
        }

        var dto = QuizDtoBuilder.Build(quiz, includeAnswerKey: isManager);

        return ApiResponse<QuizDto>.Success(dto);
    }
}
