using AutoMapper;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.StudentReports.Commands;
using LearningPlatform.Application.Features.StudentReports.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.StudentReports.Handlers;

public class CreateStudentReportCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    : IRequestHandler<CreateStudentReportCommand, ApiResponse<StudentReportDto>>
{
    public async Task<ApiResponse<StudentReportDto>> Handle(CreateStudentReportCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
            throw new UnauthorizedException("You must be signed in.");

        var report = new StudentReport
        {
            Title = request.Title,
            Content = request.Content,
            InstructorId = currentUser.UserId.Value,
            StudentId = request.StudentId
        };

        await unitOfWork.Repository<StudentReport>().AddAsync(report, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<StudentReportDto>(report);
        dto.InstructorName = currentUser.UserName ?? "Instructor";

        return ApiResponse<StudentReportDto>.Success(dto, "Report created successfully.");
    }
}
