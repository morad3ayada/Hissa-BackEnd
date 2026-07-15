using LearningPlatform.Application.Features.StudentReports.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.StudentReports.Commands;

public class CreateStudentReportCommand : IRequest<ApiResponse<StudentReportDto>>
{
    public Guid StudentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
