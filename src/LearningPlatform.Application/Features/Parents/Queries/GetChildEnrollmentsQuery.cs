using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Parents.Queries;

public record GetChildEnrollmentsQuery(Guid StudentId) : IRequest<ApiResponse<List<EnrollmentDto>>>;
