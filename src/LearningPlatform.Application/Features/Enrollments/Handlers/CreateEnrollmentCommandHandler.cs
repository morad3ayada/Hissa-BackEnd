using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Enrollments.Commands;
using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Enrollments.Handlers;

public class CreateEnrollmentCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<CreateEnrollmentCommand, ApiResponse<EnrollmentDto>>
{
    public async Task<ApiResponse<EnrollmentDto>> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var course = await unitOfWork.Repository<Course>().GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        if (course.Status != CourseStatus.Published)
            throw new BadRequestException("You can only enroll in a published course.");

        var studentId = currentUser.UserId!.Value;
        var enrollmentRepository = unitOfWork.Repository<Enrollment>();

        var alreadyEnrolled = await enrollmentRepository.ExistsAsync(
            e => e.StudentId == studentId && e.CourseId == request.CourseId, cancellationToken);

        if (alreadyEnrolled)
            throw new ConflictException("You are already enrolled in this course.");

        var effectivePrice = course.DiscountPrice ?? course.Price;
        var isFree = effectivePrice <= 0;

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = request.CourseId,
            EnrolledAt = DateTime.UtcNow,
            // Free courses skip the manual payment workflow entirely.
            Status = isFree ? EnrollmentStatus.Active : EnrollmentStatus.PendingPayment
        };

        await enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (isFree)
        {
            // A zero-amount, pre-completed payment record so lesson-access checks (which
            // require a Completed payment) work uniformly for free and paid courses alike.
            await unitOfWork.Repository<Payment>().AddAsync(new Payment
            {
                StudentId = studentId,
                EnrollmentId = enrollment.Id,
                Amount = 0,
                PaymentMethod = "Free",
                Status = PaymentStatus.Completed,
                PaidAt = DateTime.UtcNow
            }, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var result = await enrollmentRepository.AsQueryable()
            .Include(e => e.Course)
            .Include(e => e.Student)
            .FirstAsync(e => e.Id == enrollment.Id, cancellationToken);

        var message = enrollment.Status == EnrollmentStatus.Active
            ? "Enrolled successfully."
            : "Enrollment created. Submit a payment to activate it.";

        return ApiResponse<EnrollmentDto>.Success(mapper.Map<EnrollmentDto>(result), message);
    }
}
