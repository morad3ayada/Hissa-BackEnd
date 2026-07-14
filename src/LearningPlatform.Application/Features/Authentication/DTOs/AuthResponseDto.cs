using AutoMapper;
using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Authentication.DTOs;

public class AuthResponseDto : IMapFrom<ApplicationUser>
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ApplicationUser, AuthResponseDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.FullName, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()))
            .ForMember(d => d.AccessToken, o => o.Ignore())
            .ForMember(d => d.RefreshToken, o => o.Ignore())
            .ForMember(d => d.ExpiresAt, o => o.Ignore());
    }
}
