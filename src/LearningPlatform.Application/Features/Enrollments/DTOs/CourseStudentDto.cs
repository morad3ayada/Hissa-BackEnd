using AutoMapper;
using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Enrollments.DTOs;

public class CourseStudentDto : IMapFrom<Enrollment>
{
    public Guid EnrollmentId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Enrollment, CourseStudentDto>()
            .ForMember(d => d.EnrollmentId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.StudentName, o => o.MapFrom(s => $"{s.Student.FirstName} {s.Student.LastName}"))
            .ForMember(d => d.StudentEmail, o => o.MapFrom(s => s.Student.Email))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
