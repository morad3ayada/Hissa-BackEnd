using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Lessons.DTOs;
using LearningPlatform.Application.Features.Lessons.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Lessons.Handlers;

public class GetSectionLessonsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetSectionLessonsQuery, ApiResponse<List<LessonDto>>>
{
    public async Task<ApiResponse<List<LessonDto>>> Handle(GetSectionLessonsQuery request, CancellationToken cancellationToken)
    {
        var lessons = await unitOfWork.Repository<Lesson>()
            .AsQueryable()
            .Where(l => l.CourseSectionId == request.SectionId)
            .OrderBy(l => l.Order)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<LessonDto>>.Success(mapper.Map<List<LessonDto>>(lessons));
    }
}