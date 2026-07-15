using Asp.Versioning;
using LearningPlatform.Application.Features.Payments.Commands;
using LearningPlatform.Application.Features.Payments.DTOs;
using LearningPlatform.Application.Features.Payments.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    private const long MaxReceiptImageSizeInBytes = 5 * 1024 * 1024;

    [HttpPost("CreatePayment")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxReceiptImageSizeInBytes)]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePayment(
        [FromForm] Guid courseId,
        [FromForm] string paymentMethod,
        [FromForm] string transactionNumber,
        [FromForm] string? notes,
        IFormFile receiptImage)
    {
        await using var stream = receiptImage.OpenReadStream();

        var result = await mediator.Send(new CreatePaymentCommand
        {
            CourseId = courseId,
            PaymentMethod = paymentMethod,
            TransactionNumber = transactionNumber,
            Notes = notes,
            ReceiptImageStream = stream,
            ReceiptImageFileName = receiptImage.FileName,
            ReceiptImageContentType = receiptImage.ContentType,
            ReceiptImageSize = receiptImage.Length
        });

        return Ok(result);
    }

    [HttpGet("MyPayments")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(PaginatedResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyPayments([FromQuery] GetMyPaymentsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetPendingPayments")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Instructor)}")]
    [ProducesResponseType(typeof(PaginatedResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingPayments([FromQuery] GetPendingPaymentsQuery query)
    {
        if (User.IsInRole(nameof(UserRole.Instructor)))
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdStr, out var userId))
            {
                query = query with { InstructorId = userId };
            }
        }
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("Approve")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Instructor)}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Approve([FromBody] ApprovePaymentCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("Reject")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Instructor)}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reject([FromBody] RejectPaymentCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetPaymentDetails/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentDetails(Guid id)
    {
        var result = await mediator.Send(new GetPaymentDetailsQuery(id));
        return Ok(result);
    }

    [HttpGet("GetReceiptImage/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReceiptImage(Guid id)
    {
        var result = await mediator.Send(new GetPaymentReceiptQuery(id));
        return File(result.Stream, result.ContentType, result.FileName);
    }
}
