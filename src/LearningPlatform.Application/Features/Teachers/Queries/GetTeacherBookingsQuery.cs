using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Queries;

public record GetTeacherBookingsQuery : IRequest<ApiResponse<List<BookingDto>>>
{
    public BookingStatus? Status { get; init; }
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
