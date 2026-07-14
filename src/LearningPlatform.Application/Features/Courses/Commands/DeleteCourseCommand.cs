using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Courses.Commands;

public record DeleteCourseCommand(Guid Id) : IRequest<ApiResponse>;
