using LearningPlatform.Application.Features.Reports.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Reports.Queries;

public record GetStudentsReportQuery : IRequest<ApiResponse<StudentsReportDto>>;
