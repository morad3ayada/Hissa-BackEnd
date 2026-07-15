using LearningPlatform.Application.Features.Certificates.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Certificates.Commands;

public record IssueCertificateCommand(Guid StudentId, Guid CourseId) : IRequest<ApiResponse<CertificateDto>>;
