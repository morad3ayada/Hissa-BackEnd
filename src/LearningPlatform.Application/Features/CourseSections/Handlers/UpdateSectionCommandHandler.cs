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

public class UpdateSectionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    : IRequestHandler<UpdateSectionCommand, ApiResponse<SectionDto>>
{
    public async Task<ApiResponse<SectionDto>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var sectionRepository = unitOfWork.Repository<CourseSection>();

        var section = await sectionRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(CourseSection), request.Id);

        var course = await unitOfWork.Repository<Course>().GetByIdAsync(section.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), section.CourseId);

        currentUser.EnsureCanManageCourse(course);

        section.Title = request.Title;
        section.Order = request.Order;

        sectionRepository.Update(section);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<SectionDto>.Success(mapper.Map<SectionDto>(section), "Section updated successfully.");
    }
}
