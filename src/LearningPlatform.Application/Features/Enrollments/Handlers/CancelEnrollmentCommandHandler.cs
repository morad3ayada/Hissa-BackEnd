using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Enrollments.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Handlers;

public class CancelEnrollmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<CancelEnrollmentCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(CancelEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;
        var repository = unitOfWork.Repository<Enrollment>();

        var enrollments = await repository.FindAsync(
            e => e.CourseId == request.CourseId && e.StudentId == studentId, cancellationToken);
        var enrollment = enrollments.FirstOrDefault()
            ?? throw new NotFoundException("You are not enrolled in this course.");

        // Once a payment has been approved (Active) the subscription can no longer be
        // self-cancelled here; that requires a refund process, which is out of scope.
        if (enrollment.Status != EnrollmentStatus.PendingPayment)
            throw new BadRequestException($"An enrollment with status '{enrollment.Status}' cannot be cancelled.");

        enrollment.Status = EnrollmentStatus.Cancelled;
        repository.Update(enrollment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Enrollment cancelled successfully.");
    }
}
