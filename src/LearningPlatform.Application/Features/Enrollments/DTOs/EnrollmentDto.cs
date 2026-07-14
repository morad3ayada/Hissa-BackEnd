using AutoMapper;
using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Enrollments.DTOs;

public class EnrollmentDto : IMapFrom<Enrollment>
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string? CourseThumbnailUrl { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(d => d.CourseTitle, o => o.MapFrom(s => s.Course.Title))
            .ForMember(d => d.CourseThumbnailUrl, o => o.MapFrom(s => s.Course.ThumbnailUrl))
            .ForMember(d => d.StudentName, o => o.MapFrom(s => $"{s.Student.FirstName} {s.Student.LastName}"))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
