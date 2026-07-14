using LearningPlatform.Application.Features.CourseSections.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.CourseSections.Queries;

public record GetSectionsQuery(Guid CourseId) : IRequest<ApiResponse<List<SectionDto>>>;
