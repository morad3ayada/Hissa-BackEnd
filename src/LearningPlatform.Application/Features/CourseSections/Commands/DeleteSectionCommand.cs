using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.CourseSections.Commands;

public record DeleteSectionCommand(Guid Id) : IRequest<ApiResponse>;
