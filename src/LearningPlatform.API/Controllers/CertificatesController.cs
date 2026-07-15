using Asp.Versioning;
using LearningPlatform.Application.Features.Certificates.DTOs;
using LearningPlatform.Application.Features.Certificates.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CertificatesController(IMediator mediator) : ControllerBase
{
    [HttpGet("MyCertificates")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(ApiResponse<List<CertificateDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyCertificates()
    {
        var result = await mediator.Send(new GetMyCertificatesQuery());
        return Ok(result);
    }

    [HttpPost("MyCertificates")]
    [HttpPost("Issue")]
    [Authorize(Roles = $"{nameof(UserRole.Instructor)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> IssueCertificate([FromBody] LearningPlatform.Application.Features.Certificates.Commands.IssueCertificateCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetCertificateByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("Download/{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Download(Guid id)
    {
        var result = await mediator.Send(new DownloadCertificateQuery(id));
        return File(result.Stream, result.ContentType, result.FileName);
    }

    [HttpGet("Verify/{certificateNumber}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Verify(string certificateNumber)
    {
        var result = await mediator.Send(new VerifyCertificateQuery(certificateNumber));
        return Ok(result);
    }
}
