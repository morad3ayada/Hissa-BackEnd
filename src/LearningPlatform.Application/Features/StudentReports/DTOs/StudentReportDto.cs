using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.StudentReports.DTOs;

public class StudentReportDto : IMapFrom<StudentReport>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public Guid StudentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<StudentReport, StudentReportDto>()
            .ForMember(d => d.InstructorName, o => o.MapFrom(s => $"{s.Instructor.FirstName} {s.Instructor.LastName}"));
    }
}
