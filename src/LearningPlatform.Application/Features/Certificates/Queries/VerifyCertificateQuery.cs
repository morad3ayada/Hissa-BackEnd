using LearningPlatform.Application.Features.Certificates.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Certificates.Queries;

public record VerifyCertificateQuery(string CertificateNumber) : IRequest<ApiResponse<CertificateDto>>;
