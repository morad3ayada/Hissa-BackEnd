using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public DifficultyLevel Level { get; set; } = DifficultyLevel.Beginner;
    public string Language { get; set; } = "ar";
    public int? DurationInMinutes { get; set; }
    public DateTime? PublishedAt { get; set; }

    public Guid InstructorId { get; set; }
    public ApplicationUser Instructor { get; set; } = null!;

    public ICollection<CourseSection> CourseSections { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Quiz> Quizzes { get; set; } = [];
    public ICollection<Certificate> Certificates { get; set; } = [];
    public ICollection<LiveSession> LiveSessions { get; set; } = [];
}
