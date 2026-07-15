using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Parents.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Parents.Handlers;

public class LinkMyStudentCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<LinkMyStudentCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(LinkMyStudentCommand request, CancellationToken cancellationToken)
    {
        var parent = await userManager.FindByIdAsync(currentUser.UserId!.Value.ToString())
            ?? throw new NotFoundException("Parent not found.");

        var student = await userManager.FindByEmailAsync(request.StudentEmail)
            ?? throw new NotFoundException("Student not found.");

        if (student.Role != UserRole.Student)
            throw new BadRequestException("The specified user is not a Student account.");

        var alreadyLinked = await unitOfWork.Repository<ParentStudent>().ExistsAsync(
            ps => ps.ParentId == parent.Id && ps.StudentId == student.Id, cancellationToken);

        if (alreadyLinked)
            throw new ConflictException("You are already linked to this student.");

        await unitOfWork.Repository<ParentStudent>().AddAsync(new ParentStudent
        {
            ParentId = parent.Id,
            StudentId = student.Id,
            RelationshipType = request.RelationshipType,
            LinkedAt = DateTime.UtcNow
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success($"Linked to {student.FirstName} {student.LastName} successfully.");
    }
}
