using LearningPlatform.Application.Features.Reports.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Reports.Queries;

public record GetQuizzesReportQuery : IRequest<ApiResponse<QuizzesReportDto>>;
