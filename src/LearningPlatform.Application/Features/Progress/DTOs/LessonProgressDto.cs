using AutoMapper;
using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Progress.DTOs;

public class LessonProgressDto : IMapFrom<CourseProgress>
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public int CurrentSecond { get; set; }
    public decimal WatchPercentage { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? LastWatchedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CourseProgress, LessonProgressDto>()
            .ForMember(d => d.LessonTitle, o => o.MapFrom(s => s.Lesson.Title));
    }
}
