using LearningPlatform.Application.Features.Parents.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Parents.Queries;

public record GetMyChildrenQuery : IRequest<ApiResponse<List<ChildDto>>>;
