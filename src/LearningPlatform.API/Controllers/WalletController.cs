using Asp.Versioning;
using LearningPlatform.Application.Features.Wallet.Commands;
using LearningPlatform.Application.Features.Wallet.DTOs;
using LearningPlatform.Application.Features.Wallet.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class WalletController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<WalletSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary()
    {
        var result = await mediator.Send(new GetWalletSummaryQuery());
        return Ok(result);
    }

    [HttpGet("transactions")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(PaginatedResponse<WalletTransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions([FromQuery] GetWalletTransactionsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("requests")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(PaginatedResponse<WalletRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyRequests([FromQuery] GetMyWalletRequestsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("deposits")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<WalletRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateDepositRequest([FromBody] CreateDepositRequestRequest request)
    {
        var result = await mediator.Send(new CreateDepositRequestCommand(request.Amount, request.Notes));
        return Ok(result);
    }

    [HttpPost("withdrawals")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<WalletRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateWithdrawalRequest([FromBody] CreateWithdrawalRequestRequest request)
    {
        var result = await mediator.Send(new CreateWithdrawalRequestCommand(request.Amount));
        return Ok(result);
    }

    [HttpGet("admin/requests")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(PaginatedResponse<WalletRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRequests([FromQuery] GetAllWalletRequestsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("admin/requests/{requestId:guid}/approve")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse<WalletRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveRequest(Guid requestId)
    {
        var result = await mediator.Send(new ApproveWalletRequestCommand(requestId));
        return Ok(result);
    }

    [HttpPut("admin/requests/{requestId:guid}/reject")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ApiResponse<WalletRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectRequest(Guid requestId, [FromBody] RejectWalletRequestRequest request)
    {
        var result = await mediator.Send(new RejectWalletRequestCommand(requestId, request.Reason));
        return Ok(result);
    }

    public record CreateDepositRequestRequest(decimal Amount, string? Notes);

    public record CreateWithdrawalRequestRequest(decimal Amount);

    public record RejectWalletRequestRequest(string Reason);
}
