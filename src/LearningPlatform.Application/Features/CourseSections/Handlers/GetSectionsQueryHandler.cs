using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.CourseSections.DTOs;
using LearningPlatform.Application.Features.CourseSections.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.CourseSections.Handlers;

public class GetSectionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetSectionsQuery, ApiResponse<List<SectionDto>>>
{
    public async Task<ApiResponse<List<SectionDto>>> Handle(GetSectionsQuery request, CancellationToken cancellationToken)
    {
        _ = await unitOfWork.Repository<Course>().GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        var sections = await unitOfWork.Repository<CourseSection>()
            .AsQueryable()
            .Include(s => s.Lessons)
            .Where(s => s.CourseId == request.CourseId)
            .OrderBy(s => s.Order)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<SectionDto>>.Success(mapper.Map<List<SectionDto>>(sections));
    }
}