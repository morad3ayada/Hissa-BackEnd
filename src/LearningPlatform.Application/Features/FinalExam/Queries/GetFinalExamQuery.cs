using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.FinalExam.Queries;

public record GetFinalExamQuery(Guid CourseId) : IRequest<ApiResponse<QuizDto>>;
