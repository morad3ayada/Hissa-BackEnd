using AutoMapper;
using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.CourseSections.Commands;
using LearningPlatform.Application.Features.CourseSections.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.CourseSections.Handlers;

public class CreateSectionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    : IRequestHandler<CreateSectionCommand, ApiResponse<SectionDto>>
{
    public async Task<ApiResponse<SectionDto>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var course = await unitOfWork.Repository<Course>().GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        currentUser.EnsureCanManageCourse(course);

        var sectionRepository = unitOfWork.Repository<CourseSection>();
        var existingSections = await sectionRepository.FindAsync(s => s.CourseId == request.CourseId, cancellationToken);
        var nextOrder = existingSections.Count == 0 ? 1 : existingSections.Max(s => s.Order) + 1;

        var section = new CourseSection
        {
            CourseId = request.CourseId,
            Title = request.Title,
            Order = nextOrder
        };

        await sectionRepository.AddAsync(section, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<SectionDto>.Success(mapper.Map<SectionDto>(section), "Section created successfully.");
    }
}
