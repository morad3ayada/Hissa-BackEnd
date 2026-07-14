using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Progress.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Common.Services;

public class CourseCompletionService(
    IUnitOfWork unitOfWork,
    ICertificatePdfGenerator pdfGenerator,
    IFileStorageService fileStorageService,
    ICourseProgressCalculator courseProgressCalculator)
    : ICourseCompletionService
{
    public async Task TryGrantCourseCompletionAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        // Deliberately no .Include() here: this instance is later passed to Update(), and EF's
        // Update() walks the whole reachable graph. If Student/Course were loaded, and a Course
        // with the same Id is already tracked elsewhere in this request (very likely — callers
        // like SubmitQuizCommandHandler/UpdateLessonProgressCommandHandler load it via tracked
        // GetByIdAsync earlier in their own flow), attaching a second untracked Course instance
        // with that Id throws "already being tracked". Names for the PDF are fetched separately
        // below via a pure projection, which EF never tracks regardless of Include.
        var enrollment = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .FirstOrDefaultAsync(
                e => e.StudentId == studentId && e.CourseId == courseId && e.Status == EnrollmentStatus.Active,
                cancellationToken);

        if (enrollment is null)
            return;

        var alreadyCertified = await unitOfWork.Repository<Certificate>()
            .ExistsAsync(c => c.EnrollmentId == enrollment.Id, cancellationToken);

        if (alreadyCertified)
            return;

        var hasFinalExam = await unitOfWork.Repository<Quiz>().ExistsAsync(
            q => q.CourseId == courseId && q.IsFinalExam, cancellationToken);

        if (hasFinalExam)
        {
            var passedFinalExam = await unitOfWork.Repository<QuizResult>().AsQueryable()
                .Include(r => r.Quiz)
                .AnyAsync(
                    r => r.StudentId == studentId && r.IsPassed && r.Quiz.CourseId == courseId && r.Quiz.IsFinalExam,
                    cancellationToken);

            if (!passedFinalExam)
                return;
        }
        else
        {
            var summary = await courseProgressCalculator.CalculateSummaryAsync(courseId, studentId, cancellationToken);
            if (summary.CompletionPercentage < 100)
                return;
        }

        enrollment.Status = EnrollmentStatus.Completed;
        enrollment.CompletedAt = DateTime.UtcNow;
        unitOfWork.Repository<Enrollment>().Update(enrollment);

        // Pure projection, not an entity query — EF never tracks anonymous-type results, so this
        // cannot collide with whatever Course/Student instances the caller already has tracked.
        var names = await unitOfWork.Repository<Enrollment>().AsQueryable()
            .Where(e => e.Id == enrollment.Id)
            .Select(e => new
            {
                StudentName = e.Student.FirstName + " " + e.Student.LastName,
                CourseTitle = e.Course.Title,
                InstructorName = e.Course.Instructor.FirstName + " " + e.Course.Instructor.LastName
            })
            .FirstAsync(cancellationToken);

        var certificateNumber = $"CERT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        var issuedAt = DateTime.UtcNow;

        var pdfBytes = pdfGenerator.Generate(new CertificatePdfData(
            certificateNumber,
            names.StudentName,
            names.CourseTitle,
            names.InstructorName,
            issuedAt));

        using var pdfStream = new MemoryStream(pdfBytes);
        var certificateUrl = await fileStorageService.UploadAsync(
            pdfStream, $"Certificates/{studentId}/{certificateNumber}.pdf", "application/pdf", cancellationToken);

        await unitOfWork.Repository<Certificate>().AddAsync(new Certificate
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrollmentId = enrollment.Id,
            IssuedAt = issuedAt,
            CertificateNumber = certificateNumber,
            CertificateUrl = certificateUrl
        }, cancellationToken);
    }
}
