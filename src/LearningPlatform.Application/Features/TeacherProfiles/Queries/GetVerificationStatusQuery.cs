using LearningPlatform.Application.Features.TeacherProfiles.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.TeacherProfiles.Queries;

public record GetVerificationStatusQuery : IRequest<ApiResponse<VerificationStatusDto>>;
