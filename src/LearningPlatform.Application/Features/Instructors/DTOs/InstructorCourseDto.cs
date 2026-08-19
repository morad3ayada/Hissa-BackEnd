namespace LearningPlatform.Application.Features.Instructors.DTOs;

public class InstructorCourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string Level { get; set; } = string.Empty;
    public int? DurationInMinutes { get; set; }
}
