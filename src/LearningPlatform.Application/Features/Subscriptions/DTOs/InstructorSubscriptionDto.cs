using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Subscriptions.DTOs;

public class InstructorSubscriptionDto : IMapFrom<InstructorSubscription>
{
    public Guid Id { get; set; }
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public Guid PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SubscriptionStatus Status { get; set; }
    public bool IsActive => Status == SubscriptionStatus.Active && EndDate > DateTime.UtcNow;

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<InstructorSubscription, InstructorSubscriptionDto>()
            .ForMember(d => d.InstructorName, o => o.MapFrom(s => $"{s.Instructor.FirstName} {s.Instructor.LastName}"))
            .ForMember(d => d.PlanName, o => o.MapFrom(s => s.Plan.Name));
    }
}
