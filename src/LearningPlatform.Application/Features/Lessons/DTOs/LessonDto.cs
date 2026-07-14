using AutoMapper;
using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Lessons.DTOs;

public class LessonDto : IMapFrom<Lesson>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Content { get; set; }
    public bool HasVideo { get; set; }
    public int? DurationInSeconds { get; set; }
    public int Order { get; set; }
    public bool IsFreePreview { get; set; }
    public Guid CourseSectionId { get; set; }
    public DateTime CreatedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Lesson, LessonDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.HasVideo, o => o.MapFrom(s => !string.IsNullOrEmpty(s.ContentUrl)));
    }
}
