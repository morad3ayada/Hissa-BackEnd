namespace LearningPlatform.Application.Features.Progress.DTOs;

public class CourseProgressDetailDto : CourseProgressSummaryDto
{
    public List<LessonProgressDto> Lessons { get; set; } = [];
}
