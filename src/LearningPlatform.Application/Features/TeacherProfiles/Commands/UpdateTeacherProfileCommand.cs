using LearningPlatform.Application.Features.TeacherProfiles.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.TeacherProfiles.Commands;

public record UpdateTeacherProfileCommand : IRequest<ApiResponse<TeacherProfileDto>>
{
    public string? ProfileImageUrl { get; init; }
    public string RealName { get; init; } = string.Empty;
    public string? Specialization { get; init; }
    public List<string> Subjects { get; init; } = [];
    public List<string> Grades { get; init; } = [];
    public string? Governorate { get; init; }
    public int? YearsOfExperience { get; init; }
    public string? Bio { get; init; }
    public decimal? LessonPrice { get; init; }
    public List<string> Certificates { get; init; } = [];
    public List<string> Qualifications { get; init; } = [];
    public List<string> RequiredDocuments { get; init; } = [];
}
