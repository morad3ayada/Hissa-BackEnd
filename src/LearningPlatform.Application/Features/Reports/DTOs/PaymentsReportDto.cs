using LearningPlatform.Application.Features.Dashboard.DTOs;

namespace LearningPlatform.Application.Features.Reports.DTOs;

public class PaymentsReportDto
{
    public List<PaymentStatusBreakdownDto> ByStatus { get; set; } = [];
    public List<PaymentMethodBreakdownDto> ByMethod { get; set; } = [];
    public decimal TotalRevenue { get; set; }
}

public class PaymentMethodBreakdownDto
{
    public string Method { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}
