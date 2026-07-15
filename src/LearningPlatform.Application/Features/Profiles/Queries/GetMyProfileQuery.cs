using LearningPlatform.Application.Features.Profiles.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Profiles.Queries;

public record GetMyProfileQuery : IRequest<ApiResponse<ProfileDto>>;
