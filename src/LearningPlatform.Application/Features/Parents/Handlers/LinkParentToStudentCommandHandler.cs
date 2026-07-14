using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Parents.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Parents.Handlers;

public class LinkParentToStudentCommandHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    : IRequestHandler<LinkParentToStudentCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(LinkParentToStudentCommand request, CancellationToken cancellationToken)
    {
        var parent = await userManager.FindByIdAsync(request.ParentId.ToString())
            ?? throw new NotFoundException(nameof(ApplicationUser), request.ParentId);

        if (parent.Role != UserRole.Parent)
            throw new BadRequestException("The specified user is not a Parent account.");

        var student = await userManager.FindByIdAsync(request.StudentId.ToString())
            ?? throw new NotFoundException(nameof(ApplicationUser), request.StudentId);

        if (student.Role != UserRole.Student)
            throw new BadRequestException("The specified user is not a Student account.");

        var alreadyLinked = await unitOfWork.Repository<ParentStudent>().ExistsAsync(
            ps => ps.ParentId == request.ParentId && ps.StudentId == request.StudentId, cancellationToken);

        if (alreadyLinked)
            throw new ConflictException("This parent is already linked to this student.");

        await unitOfWork.Repository<ParentStudent>().AddAsync(new ParentStudent
        {
            ParentId = request.ParentId,
            StudentId = request.StudentId,
            RelationshipType = request.RelationshipType,
            LinkedAt = DateTime.UtcNow
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success($"Linked {parent.FirstName} {parent.LastName} to {student.FirstName} {student.LastName}.");
    }
}
