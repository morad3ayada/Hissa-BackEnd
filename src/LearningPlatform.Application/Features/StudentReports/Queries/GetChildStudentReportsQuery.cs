using LearningPlatform.Application.Features.StudentReports.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.StudentReports.Queries;

public record GetChildStudentReportsQuery(Guid StudentId) : IRequest<ApiResponse<List<StudentReportDto>>>;
