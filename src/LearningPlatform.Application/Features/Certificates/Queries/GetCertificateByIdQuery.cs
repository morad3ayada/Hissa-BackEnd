using LearningPlatform.Application.Features.Certificates.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Certificates.Queries;

public record GetCertificateByIdQuery(Guid Id) : IRequest<ApiResponse<CertificateDto>>;
