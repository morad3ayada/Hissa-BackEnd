using LearningPlatform.Application.Features.Dashboard.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Dashboard.Queries;

public record GetInstructorDashboardQuery : IRequest<ApiResponse<InstructorDashboardDto>>;
