using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Queries;

public record GetStudentEnrollmentsQuery(Guid StudentId) : IRequest<ApiResponse<List<EnrollmentDto>>>;