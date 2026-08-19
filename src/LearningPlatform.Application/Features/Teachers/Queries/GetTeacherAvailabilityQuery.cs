using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Queries;

public record GetTeacherAvailabilityQuery : IRequest<ApiResponse<List<TeacherAvailabilityDto>>>;
