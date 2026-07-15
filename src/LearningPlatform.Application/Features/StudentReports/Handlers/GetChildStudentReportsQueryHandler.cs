using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.StudentReports.DTOs;
using LearningPlatform.Application.Features.StudentReports.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.StudentReports.Handlers;

public class GetChildStudentReportsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    : IRequestHandler<GetChildStudentReportsQuery, ApiResponse<List<StudentReportDto>>>
{
    public async Task<ApiResponse<List<StudentReportDto>>> Handle(GetChildStudentReportsQuery request, CancellationToken cancellationToken)
    {
        var isParent = currentUser.IsInRole("Parent");
        var isInstructor = currentUser.IsInRole("Instructor");
        var isAdmin = currentUser.IsInRole("Admin");

        if (isParent)
        {
            var isLinked = await unitOfWork.Repository<ParentStudent>().ExistsAsync(
                ps => ps.ParentId == currentUser.UserId && ps.StudentId == request.StudentId,
                cancellationToken);

            if (!isLinked)
                throw new ForbiddenException("This student is not one of your children.");
        }
        else if (!isAdmin)
        {
            var teachesCourse = await unitOfWork.Repository<Course>().ExistsAsync(
                c => c.InstructorId == currentUser.UserId && c.Enrollments.Any(e => e.StudentId == request.StudentId),
                cancellationToken);

            if (!teachesCourse)
                throw new ForbiddenException("You can only view reports for students in your courses.");
        }

        var reports = await unitOfWork.Repository<StudentReport>().AsQueryable()
            .Include(r => r.Instructor)
            .Where(r => r.StudentId == request.StudentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = mapper.Map<List<StudentReportDto>>(reports);
        return ApiResponse<List<StudentReportDto>>.Success(dtos);
    }
}
