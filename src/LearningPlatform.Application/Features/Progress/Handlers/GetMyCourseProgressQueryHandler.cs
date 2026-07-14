using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Progress.DTOs;
using LearningPlatform.Application.Features.Progress.Interfaces;
using LearningPlatform.Application.Features.Progress.Queries;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Progress.Handlers;

public class GetMyCourseProgressQueryHandler(ICourseProgressCalculator calculator, ICurrentUserService currentUser)
    : IRequestHandler<GetMyCourseProgressQuery, ApiResponse<CourseProgressDetailDto>>
{
    public async Task<ApiResponse<CourseProgressDetailDto>> Handle(GetMyCourseProgressQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;
        var result = await calculator.CalculateDetailAsync(request.CourseId, studentId, cancellationToken);

        return ApiResponse<CourseProgressDetailDto>.Success(result);
    }
}
