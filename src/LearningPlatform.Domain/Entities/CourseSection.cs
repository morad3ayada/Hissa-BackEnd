using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class CourseSection : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<Lesson> Lessons { get; set; } = [];
}
