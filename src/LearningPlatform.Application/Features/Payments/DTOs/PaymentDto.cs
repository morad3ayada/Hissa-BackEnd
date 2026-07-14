using AutoMapper;
using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Payments.DTOs;

public class PaymentDto : IMapFrom<Payment>
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public string? ReceiptImageUrl { get; set; }
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Payment, PaymentDto>()
            .ForMember(d => d.CourseId, o => o.MapFrom(s => s.Enrollment.CourseId))
            .ForMember(d => d.CourseTitle, o => o.MapFrom(s => s.Enrollment.Course.Title))
            .ForMember(d => d.StudentName, o => o.MapFrom(s => $"{s.Student.FirstName} {s.Student.LastName}"))
            .ForMember(d => d.StudentEmail, o => o.MapFrom(s => s.Student.Email))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
