using LearningPlatform.Application.Features.Progress.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Progress.Queries;

public record GetStudentProgressQuery(Guid StudentId) : IRequest<ApiResponse<List<CourseProgressSummaryDto>>>;
