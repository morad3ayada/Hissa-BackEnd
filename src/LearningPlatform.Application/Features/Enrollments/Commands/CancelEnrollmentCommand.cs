using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Commands;

public record CancelEnrollmentCommand(Guid CourseId) : IRequest<ApiResponse>;
