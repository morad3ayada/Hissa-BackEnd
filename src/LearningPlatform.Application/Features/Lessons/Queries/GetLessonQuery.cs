using LearningPlatform.Application.Features.Lessons.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Lessons.Queries;

public record GetLessonQuery(Guid Id) : IRequest<ApiResponse<LessonDto>>;
