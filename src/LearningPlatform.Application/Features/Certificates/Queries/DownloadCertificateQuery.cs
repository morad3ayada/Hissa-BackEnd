using MediatR;

namespace LearningPlatform.Application.Features.Certificates.Queries;

public record DownloadCertificateQuery(Guid Id) : IRequest<CertificateFileResult>;

public record CertificateFileResult(Stream Stream, string ContentType, string FileName);
