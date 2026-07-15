using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Parents.DTOs;
using LearningPlatform.Application.Features.Parents.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Parents.Handlers;

public class GetMyChildrenQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetMyChildrenQuery, ApiResponse<List<ChildDto>>>
{
    public async Task<ApiResponse<List<ChildDto>>> Handle(GetMyChildrenQuery request, CancellationToken cancellationToken)
    {
        var parentId = currentUser.UserId!.Value;

        var children = await unitOfWork.Repository<ParentStudent>()
            .AsQueryable()
            .Where(ps => ps.ParentId == parentId)
            .Select(ps => ps.Student)
            .ToListAsync(cancellationToken);

        var dtos = mapper.Map<List<ChildDto>>(children);
        return ApiResponse<List<ChildDto>>.Success(dtos);
    }
}
