using LearningPlatform.Application.Features.Progress.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Progress.Queries;

public record GetMyCourseProgressQuery(Guid CourseId) : IRequest<ApiResponse<CourseProgressDetailDto>>;
