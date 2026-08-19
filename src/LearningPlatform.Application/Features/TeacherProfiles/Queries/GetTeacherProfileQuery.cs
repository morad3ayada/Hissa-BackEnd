using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.TeacherProfiles.Queries;

public record GetTeacherProfileQuery : IRequest<ApiResponse<DTOs.TeacherProfileDto>>;
