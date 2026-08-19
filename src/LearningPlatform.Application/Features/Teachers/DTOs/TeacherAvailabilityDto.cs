namespace LearningPlatform.Application.Features.Teachers.DTOs;

public class TeacherAvailabilityDto
{
    public string Day { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}

public class UpdateAvailabilityRequest
{
    public List<TeacherAvailabilityDto> Availability { get; set; } = [];
}
